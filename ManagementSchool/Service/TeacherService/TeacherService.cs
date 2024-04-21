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

        // Kiểm tra điểm số nhập vào có hợp lệ không
        if (scoreDto.Value < 0 || scoreDto.Value > 10) throw new ArgumentException("Score must be between 0 and 10.");

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
                SubjectName = s.StudentSubjects.Any()
                    ? s.StudentSubjects.FirstOrDefault().Subject.SubjectName
                    : "No Subject",
                ScoreValue = s.Scores.Any() ? s.Scores.FirstOrDefault().Value : 0,
                ExamType = s.Scores.Any() ? s.Scores.FirstOrDefault().ExamType : "No Exam",
                Semester = s.Scores.Any() ? s.Scores.FirstOrDefault().SemesterName : "No Semester"
            }))
            .ToListAsync();

        return assignedClassStudents;
    }

    public async Task<SemesterScoresDto> CalculateScoreForSemestersAsync(string teacherEmail, int studentId, int subjectId)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == teacherEmail);
        if (teacher == null)
            throw new ArgumentException("Teacher not found.");

        var isAssigned = await _context.TeacherClasses
            .AnyAsync(tc => tc.TeacherId == teacher.TeacherId &&
                            tc.Class.Students.Any(s => s.StudentId == studentId) &&
                            tc.Teacher.SubjectId == subjectId);

        if (!isAssigned)
            throw new ArgumentException("Teacher is not assigned to teach the subject for this class.");

        var scores = await _context.Scores
            .Where(s => s.StudentId == studentId &&
                        s.SubjectId == subjectId &&
                        s.Student.Class.TeacherClasses.Any(tc => tc.Teacher.Email == teacherEmail))
            .ToListAsync();

        var semesterScores = new SemesterScoresDto
        {
            Semester1Score = CalculateSemesterScore(scores, "Semester 1"),
            Semester2Score = CalculateSemesterScore(scores, "Semester 2")
        };
        return semesterScores; 
    }
    
    public async Task<double?> CalculateAnnualAverageScoreAsync(string teacherEmail, int studentId, int subjectId)
    {
        var semesterScores = await CalculateScoreForSemestersAsync(teacherEmail, studentId, subjectId);
        
        if (semesterScores.Semester1Score.HasValue && semesterScores.Semester2Score.HasValue)
        {
            double annualAverage = (semesterScores.Semester1Score.Value + 2 * semesterScores.Semester2Score.Value) / 3;
            return Math.Round(annualAverage, 1);
        }
        else
        {
            return null;
        }
    }

    private double? CalculateSemesterScore(List<Score> scores, string semesterName)
    {
        var semesterScores = scores.Where(s => s.SemesterName == semesterName).ToList();
        if (semesterScores.Count == 0)
        {
            return null; // No scores for this semester
        }

        double totalScore = semesterScores.Sum(s => s.Value);
        double averageScore = totalScore / semesterScores.Count;

        // Round the average score to one decimal place
        return Math.Round(averageScore, 1);
    }



}