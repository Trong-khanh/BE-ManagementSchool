using Microsoft.EntityFrameworkCore;
using ManagementSchool.Models; // Thay thế bằng namespace chứa ApplicationDbContext của bạn
using ManagementSchool.Entities; // Thay thế bằng namespace chứa các entity của bạn
using ManagementSchool.Dto; // Thay thế bằng namespace chứa các DTO của bạn
using ManagementSchool.Service.TeacherService; // Thay thế bằng namespace chứa ITeacherService

namespace ManagementSchool.Service.TeacherService;

public class TeacherService : ITeacherService
{
    private readonly ApplicationDbContext _context;

    public TeacherService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SemesterDto>> GetAllSemestersAsync()
    {
        var semesters = await _context.Semesters.Select(s => new SemesterDto
        {
            SemesterId = s.SemesterId,
            Name = s.Name,
            StartDate = s.StartDate,
            EndDate = s.EndDate
        }).ToListAsync();

        return semesters;
    }

    public async Task AddScoreAsync(ScoreDto scoreDto, string teacherEmail)
    {
        // Kiểm tra xem giáo viên có được gán vào lớp không
        var assignedClasses = await _context.TeacherClasses
            .Where(tc => tc.Teacher.Email == teacherEmail)
            .Select(tc => tc.ClassId)
            .ToListAsync();

        if (!assignedClasses.Contains(scoreDto.ClassId))
            throw new AdminService.ValidateException("Teacher is not assigned to the selected class.");

        // Kiểm tra xem kỳ học có tồn tại không
        var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Name == scoreDto.SemesterName);
        if (semester == null) throw new AdminService.ValidateException("Semester not found.");

        // Tạo bản ghi điểm mới và lưu vào cơ sở dữ liệu
        var score = new Score
        {
            StudentId = scoreDto.StudentId,
            SubjectId = scoreDto.SubjectId,
            Value = scoreDto.Value,
            SemesterName = scoreDto.SemesterName,
            ExamType = scoreDto.ExamType
        };

        _context.Scores.Add(score);
        await _context.SaveChangesAsync();
    }

    public async Task<List<StudentInfoDto>> GetAssignedClassesStudentsAsync(string teacherEmail)
    {
        var assignedClassStudents = await _context.TeacherClasses
            .Include(tc => tc.Class)
            .ThenInclude(c => c.Students)
            .Where(tc => tc.Teacher.Email == teacherEmail)
            .SelectMany(tc => tc.Class.Students.Select(s => new StudentInfoDto
            {
                StudentFullName = s.FullName,
                ClassName = tc.Class.ClassName,
                SubjectName = s.StudentSubjects.Any() ? s.StudentSubjects.FirstOrDefault().Subject.SubjectName : "No Subject",
                ScoreValue = s.Scores.Any() ? s.Scores.FirstOrDefault().Value : 0,
                ExamType = s.Scores.Any() ? s.Scores.FirstOrDefault().ExamType : "No Exam",
                Semester = s.Scores.Any() ? s.Scores.FirstOrDefault().SemesterName : "No Semester"
            }))
            .ToListAsync();

        return assignedClassStudents;
    }
}