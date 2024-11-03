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

    public TeacherService(ApplicationDbContext context, ILogger<TeacherService> logger)
    {
        _context = context;
        _logger = logger;
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
    
    public async Task<List<ScoreDto>> GetScoresForStudentAsync(int studentId, int? subjectId = null, int? semesterId = null)
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

    // public double CalculateSemesterAverage(int studentId, int subjectId, string semesterName, ClaimsPrincipal user,
    //     string academicYear)
    // {
    //     // Lấy email từ claims trong JWT token
    //     var emailClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    //     if (emailClaim == null)
    //         throw new UnauthorizedAccessException("Email claim not found in token.");
    //
    //     var teacherEmail = emailClaim;
    //
    //     var teacher = _context.Teachers.FirstOrDefault(t => t.Email == teacherEmail && t.SubjectId == subjectId);
    //     if (teacher == null)
    //         throw new UnauthorizedAccessException("Teacher is not assigned to this subject or does not exist.");
    //
    //     var scores = _context.Scores
    //         .Where(s => s.StudentId == studentId && s.SubjectId == subjectId && s.SemesterName.Equals(semesterName) &&
    //                     s.AcademicYear == academicYear)
    //         .ToList();
    //
    //     if (!scores.Any())
    //         throw new ArgumentException("No scores found for the specified criteria.");
    //
    //     double totalScore = 0;
    //     double countTestWhenClassBegins = 0;
    //     double countFifteenMinutesTest = 0;
    //
    //     // Accumulate scores based on exam type
    //     foreach (var score in scores)
    //         switch (score.ExamType)
    //         {
    //             case "Test when class begins":
    //                 totalScore += score.Value;
    //                 countTestWhenClassBegins += 1;
    //                 break;
    //             case "15 minutes test":
    //                 totalScore += score.Value;
    //                 countFifteenMinutesTest += 1;
    //                 break;
    //             case "45 minutes test":
    //                 totalScore += 2 * score.Value;
    //                 break;
    //             case "semester test":
    //                 totalScore += 3 * score.Value;
    //                 break;
    //             default:
    //                 throw new ArgumentException("Invalid exam type encountered in score data.");
    //         }
    //
    //     var denominator = countTestWhenClassBegins + countFifteenMinutesTest + 5;
    //     if (denominator == 0)
    //         throw new InvalidOperationException("Invalid score data. Not enough tests to calculate the average.");
    //
    //     var average = totalScore / denominator;
    //     var roundedAverage = Math.Round(average, 1);
    //
    //     SaveSemesterScore(studentId, subjectId, semesterName, roundedAverage, academicYear);
    //
    //     return roundedAverage;
    // }
    //
    // public double CalculateAnnualAverage(int studentId, int subjectId, ClaimsPrincipal user, string academicYear)
    // {
    //     var emailClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    //     if (emailClaim == null)
    //         throw new UnauthorizedAccessException("Email claim not found in token.");
    //
    //     var teacherEmail = emailClaim;
    //
    //     var teacherSubject = _context.Teachers.FirstOrDefault(t => t.Email == teacherEmail && t.SubjectId == subjectId);
    //     if (teacherSubject == null)
    //         throw new UnauthorizedAccessException("Teacher is not assigned to this subject.");
    //
    //     var semester1Average = CalculateSemesterAverage(studentId, subjectId, "Semester 1", user, academicYear);
    //     var semester2Average = CalculateSemesterAverage(studentId, subjectId, "Semester 2", user, academicYear);
    //
    //     var annualAverage = (semester1Average + 2 * semester2Average) / 3;
    //     var roundedAnnualAverage = Math.Round(annualAverage, 1);
    //
    //     SaveSemesterScore(studentId, subjectId, "Annual", roundedAnnualAverage, academicYear);
    //
    //     return roundedAnnualAverage;
    // }
    //
    // private void SaveSemesterScore(int studentId, int subjectId, string semesterName, double score, string academicYear)
    // {
    //     var studentSubjectScore = _context.StudentSubjectScores
    //         .FirstOrDefault(sss =>
    //             sss.StudentId == studentId && sss.SubjectId == subjectId && sss.AcademicYear == academicYear);
    //
    //     if (studentSubjectScore == null)
    //     {
    //         studentSubjectScore = new StudentSubjectScore
    //         {
    //             StudentId = studentId,
    //             SubjectId = subjectId,
    //             AcademicYear = academicYear
    //         };
    //         _context.StudentSubjectScores.Add(studentSubjectScore);
    //     }
    //
    //     switch (semesterName.ToLower())
    //     {
    //         case "semester 1":
    //             studentSubjectScore.Semester1Score = score;
    //             break;
    //         case "semester 2":
    //             studentSubjectScore.Semester2Score = score;
    //             break;
    //         case "annual":
    //             studentSubjectScore.AnnualScore = score;
    //             break;
    //     }
    //
    //     _context.SaveChanges();
    // }
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
}