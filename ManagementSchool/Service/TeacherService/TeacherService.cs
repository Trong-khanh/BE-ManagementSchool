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

    public TeacherService(ApplicationDbContext context)
    {
        _context = context;
    }


// public async Task AddScoreAsync(ScoreDto scoreDto, string teacherEmail)
// {
//     // Bước 1: Kiểm tra sự tồn tại của giáo viên
//     var teacher = await _context.Teachers
//         .FirstOrDefaultAsync(t => t.Email == teacherEmail);
//     if (teacher == null)
//         throw new AdminService.ValidateException("Teacher does not exist.");
//
//     // Bước 2: Kiểm tra giáo viên có được giao lớp hay không
//     var isTeacherAssigned = await _context.TeacherClasses
//         .AnyAsync(tc => tc.TeacherId == teacher.TeacherId && tc.ClassId == scoreDto.ClassId);
//     if (!isTeacherAssigned)
//         throw new AdminService.ValidateException("Teacher is not assigned to the selected class.");
//
//     // Bước 3: Kiểm tra sự tồn tại của học kỳ
//     var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Name == scoreDto.SemesterName);
//     if (semester == null)
//         throw new AdminService.ValidateException("Semester not found. Please enter a valid semester.");
//
//     // Bước 4: Kiểm tra năm học có khớp với học kỳ không
//     if (semester.AcademicYear != scoreDto.AcademicYear)
//         throw new AdminService.ValidateException("Academic year does not match with the semester.");
//
//     // Bước 5: Kiểm tra giá trị điểm hợp lệ
//     if (scoreDto.Value < 0 || scoreDto.Value > 10)
//         throw new ArgumentException("Score must be between 0 and 10.");
//
//     // Bước 6: Lấy tên lớp của học sinh
//     var studentClass = await _context.Classes
//         .Where(c => c.ClassId == scoreDto.ClassId)
//         .Select(c => c.ClassName)
//         .FirstOrDefaultAsync();
//     if (studentClass == null)
//         throw new AdminService.ValidateException("Class not found for the given student.");
//
//     // Bước 7: Thêm điểm vào bảng Score
//     var score = new Score
//     {
//         StudentId = scoreDto.StudentId,
//         SubjectId = teacher.SubjectId,
//         Value = scoreDto.Value,
//         SemesterName = scoreDto.SemesterName,
//         ExamType = scoreDto.ExamType,
//         AcademicYear = scoreDto.AcademicYear,
//         ClassName = studentClass
//     };
//
//     _context.Scores.Add(score);
//     await _context.SaveChangesAsync();
// }


    public async Task<List<StudentInfoDto>> GetAssignedClassesStudentsAsync(ClaimsPrincipal user)
    {
        // Lấy TeacherId từ Claims
        var teacherEmail = user.FindFirstValue(ClaimTypes.Email);
    
        // Tìm giáo viên kèm theo các lớp và học sinh liên quan
        var teacher = await _context.Teachers
            .Include(t => t.TeacherClasses)
            .ThenInclude(tc => tc.Class)
            .ThenInclude(c => c.Students)
            .FirstOrDefaultAsync(t => t.Email == teacherEmail);

        // Nếu không tìm thấy giáo viên, trả về danh sách rỗng
        if (teacher == null)
        {
            return new List<StudentInfoDto>(); 
        }

        // Tạo danh sách học sinh từ các lớp mà giáo viên được giao
        var students = teacher.TeacherClasses
            .SelectMany(tc => tc.Class.Students.Select(s => new StudentInfoDto
            {
                StudentFullName = s.FullName,             
                ClassName = tc.Class.ClassName,             
            }))
            .ToList();

        return students;
    }

// Ví dụ hàm để lấy điểm cho học sinh
    // private double GetScoreForStudent(int studentId, int classId)
    // {
    //     // Logic để lấy điểm cho học sinh trong lớp (chưa định nghĩa trong mã nguồn)
    //     return 0; // Thay thế bằng giá trị điểm thực tế
    // }


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