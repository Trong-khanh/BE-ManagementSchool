using System.Security.Claims;
using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace ManagementSchool.Service.TeacherService;

public class TeacherService : ITeacherService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TeacherService> _logger;

    public TeacherService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentInfoDto>> GetAssignedClassesStudentsAsync(ClaimsPrincipal user)
    {
        var teacherEmail = user.FindFirstValue(ClaimTypes.Email);

        // Lấy danh sách các học sinh mà giáo viên này phụ trách, cùng thông tin lớp và môn học
        var students = await _context.TeacherClasses
            .Where(tc => tc.Teacher.Email == teacherEmail)
            .SelectMany(tc => tc.Class.Students.Select(s => new StudentInfoDto
            {
                StudentId = s.StudentId,
                StudentFullName = s.FullName,
                ClassName = tc.Class.ClassName,
                SubjectId = tc.Teacher.SubjectId,
                SubjectName = tc.Teacher.Subject.SubjectName ?? "No Subject Assigned"
            }))
            .ToListAsync();

        return students;
    }

    public async Task<IEnumerable<SemesterDto>> GetAllSemestersAsync()
    {
        return await _context.Semesters
            .Select(s => new SemesterDto
            {
                SemesterId = s.SemesterId,
                SemesterType = s.SemesterType.GetDisplayName(),
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                AcademicYear = s.AcademicYear
            })
            .ToListAsync();
    }

    public async Task AddScoreForStudentAsync(ClaimsPrincipal user, ScoreDto scoreDto)
    {
        // Lấy email của giáo viên từ ClaimsPrincipal
        var teacherEmail = user.FindFirstValue(ClaimTypes.Email);

        // Tìm giáo viên với email này và chỉ lấy các lớp mà giáo viên được phân công
        var teacher = await _context.Teachers
            .Include(t => t.TeacherClasses)
            .ThenInclude(tc => tc.Class)
            .FirstOrDefaultAsync(t => t.Email == teacherEmail);

        // Chuyển chuỗi ExamType thành enum
        var examTypeEnum = Enum.Parse<ExamType>(scoreDto.ExamType);

        // Tạo đối tượng Score để thêm vào cơ sở dữ liệu
        var score = new Score
        {
            StudentId = scoreDto.StudentId,
            SubjectId = teacher.SubjectId,
            SemesterId = scoreDto.SemesterId,
            ScoreValue = scoreDto.ScoreValue,
            ExamType = examTypeEnum
        };
        _context.Scores.Add(score);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ScoreDto>> GetScoresForStudentAsync(int studentId, int? subjectId = null,
        int? semesterId = null)
    {
        var query = _context.Scores
            .Where(s => s.StudentId == studentId)
            .AsQueryable();

        // Thêm các bộ lọc theo subjectId và semesterId nếu có
        if (subjectId.HasValue)
        {
            query = query.Where(s => s.SubjectId == subjectId);
        }

        if (semesterId.HasValue)
        {
            query = query.Where(s => s.SemesterId == semesterId);
        }

        var scores = await query
            .Select(s => new ScoreDto
            {
                StudentId = s.StudentId,
                SubjectId = s.SubjectId,
                SemesterId = s.SemesterId,
                ScoreValue = s.ScoreValue,
                ExamType = s.ExamType.ToString()
            })
            .ToListAsync();

        return scores;
    }

    public async Task<double> CalculateSemesterAverageAsync(int studentId, int semesterId, ClaimsPrincipal user)
    {
        // Lấy email từ claims trong JWT token
        var emailClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (emailClaim == null)
            throw new UnauthorizedAccessException("Email claim not found in token.");

        // Truy vấn giáo viên dựa trên email để lấy subjectId
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Email == emailClaim);
        if (teacher == null)
            throw new UnauthorizedAccessException("Teacher does not exist.");

        // Lấy subjectId từ teacher
        var subjectId = teacher.SubjectId;

        // Lấy thông tin học kỳ từ bảng Semester dựa trên semesterId
        var semester = await _context.Semesters
            .FirstOrDefaultAsync(s => s.SemesterId == semesterId);
        if (semester == null)
            throw new ArgumentException("Specified semester does not exist.");

        string academicYear = semester.AcademicYear;
        var semesterType = semester.SemesterType; // Sử dụng SemesterType để xác định kỳ học

        // Lấy điểm số của sinh viên trong học kỳ
        var scores = await _context.Scores
            .Where(s => s.StudentId == studentId && s.SubjectId == subjectId && s.SemesterId == semesterId)
            .ToListAsync();

        if (!scores.Any())
            throw new ArgumentException("No scores found for the specified criteria.");

        // Check for each required exam type
        bool hasTestWhenClassBegins = scores.Any(s => s.ExamType == ExamType.TestWhenClassBegins);
        bool hasFifteenMinutesTest = scores.Any(s => s.ExamType == ExamType.FifteenMinutesTest);
        bool hasFortyFiveMinutesTest = scores.Any(s => s.ExamType == ExamType.FortyFiveMinutesTest);
        bool hasSemesterTest = scores.Any(s => s.ExamType == ExamType.SemesterTest);

        // Collect missing exam types
        List<string> missingExamTypes = new List<string>();
        if (!hasTestWhenClassBegins) missingExamTypes.Add("TestWhenClassBegins");
        if (!hasFifteenMinutesTest) missingExamTypes.Add("FifteenMinutesTest");
        if (!hasFortyFiveMinutesTest) missingExamTypes.Add("FortyFiveMinutesTest");
        if (!hasSemesterTest) missingExamTypes.Add("SemesterTest");

        // If any exam types are missing, throw an exception
        if (missingExamTypes.Any())
        {
            string missingTypesMessage = string.Join(", ", missingExamTypes);
            throw new InvalidOperationException(
                $"Missing exam types: {missingTypesMessage}. All types are required to calculate the average.");
        }

        double totalScore = 0;
        double countTestWhenClassBegins = 0;
        double countFifteenMinutesTest = 0;

        // Tính toán tổng điểm và số lượng bài kiểm tra
        foreach (var score in scores)
        {
            switch (score.ExamType)
            {
                case ExamType.TestWhenClassBegins: // Đánh giá thường xuyên
                    totalScore += score.ScoreValue;
                    countTestWhenClassBegins += 1;
                    break;
                case ExamType.FifteenMinutesTest: // Đánh giá thường xuyên
                    totalScore += score.ScoreValue;
                    countFifteenMinutesTest += 1;
                    break;
                case ExamType.FortyFiveMinutesTest: // Đánh giá giữa kỳ
                    totalScore += 2 * score.ScoreValue;
                    break;
                case ExamType.SemesterTest: // Đánh giá cuối kỳ
                    totalScore += 3 * score.ScoreValue;
                    break;
                default:
                    throw new ArgumentException("Invalid exam type encountered in score data.");
            }
        }

        // Tính toán điểm trung bình theo quy định mới
        var denominator =
            countTestWhenClassBegins + countFifteenMinutesTest +
            5; // Hệ số của đánh giá thường xuyên + giữa kỳ (2) + cuối kỳ (3)
        if (denominator == 0)
            throw new InvalidOperationException("Invalid score data. Not enough tests to calculate the average.");

        var average = totalScore / denominator;
        var roundedAverage = Math.Round(average, 1);

        // Tìm bản ghi SubjectsAverageScore hiện tại cho học sinh, môn học, và năm học
        var subjectsAverageScore = await _context.SubjectsAverageScores
            .FirstOrDefaultAsync(s =>
                s.StudentId == studentId && s.SubjectId == subjectId && s.AcademicYear == academicYear);

        if (subjectsAverageScore == null)
        {
            // Nếu chưa có bản ghi, tạo một bản ghi mới cho cả hai kỳ học
            subjectsAverageScore = new SubjectsAverageScore
            {
                StudentId = studentId,
                SubjectId = subjectId,
                AcademicYear = academicYear,
                SemesterId = semesterId,
                SemesterAverage1 = semesterType == SemesterType.Semester1 ? roundedAverage : (double?)null,
                SemesterAverage2 = semesterType == SemesterType.Semester2 ? roundedAverage : (double?)null
            };
            await _context.SubjectsAverageScores.AddAsync(subjectsAverageScore);
        }
        else
        {
            // Nếu bản ghi đã tồn tại, chỉ cập nhật cột phù hợp dựa trên SemesterType
            if (semesterType == SemesterType.Semester1)
                subjectsAverageScore.SemesterAverage1 = roundedAverage;
            else if (semesterType == SemesterType.Semester2)
                subjectsAverageScore.SemesterAverage2 = roundedAverage;
        }

        // Tính toán điểm trung bình cả năm nếu cả hai học kỳ đều có giá trị
        if (subjectsAverageScore.SemesterAverage1.HasValue && subjectsAverageScore.SemesterAverage2.HasValue)
        {
            subjectsAverageScore.AnnualAverage = Math.Round(
                (subjectsAverageScore.SemesterAverage1.Value + subjectsAverageScore.SemesterAverage2.Value) / 2, 1);
        }

        await _context.SaveChangesAsync();

        return roundedAverage;
    }

    public async Task<SemesterAverageScoreDto> GetStudentSubjectAverageScoreAsync(int studentId, int semesterId,
        ClaimsPrincipal user)
    {
        // Lấy email từ JWT token claims
        var emailClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (emailClaim == null)
            throw new UnauthorizedAccessException("Email claim not found in token.");

        // Lấy giáo viên dựa trên email để xác định subjectId
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Email == emailClaim);
        if (teacher == null)
            throw new UnauthorizedAccessException("Teacher does not exist.");

        var subjectId = teacher.SubjectId;

        // Lấy điểm trung bình môn học cho học sinh cụ thể và học kỳ
        var subjectScore = await _context.SubjectsAverageScores
            .Where(s => s.StudentId == studentId && s.SubjectId == subjectId && s.SemesterId == semesterId)
            .Select(s => new SemesterAverageScoreDto()
            {
                SemesterAverage1 = s.SemesterAverage1,
                SemesterAverage2 = s.SemesterAverage2,
                AnnualAverage = s.AnnualAverage
            })
            .FirstOrDefaultAsync();

        if (subjectScore == null)
            throw new ArgumentException("No average score found for the specified student, subject, and semester.");
        return subjectScore;
    }
}