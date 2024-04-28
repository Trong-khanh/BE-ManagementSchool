using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ManagementSchool.Models;
using ManagementSchool.Entities;
using ManagementSchool.Dto;
using Microsoft.AspNetCore.Identity;

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
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Email == teacherEmail);

        if (teacher == null || teacher.SubjectId != scoreDto.SubjectId)
            throw new AdminService.ValidateException("Teacher is not assigned to this subject or does not exist.");

        var assignedClasses = await _context.TeacherClasses
            .Where(tc => tc.Teacher.Email == teacherEmail)
            .Select(tc => tc.ClassId)
            .ToListAsync();

        if (!assignedClasses.Contains(scoreDto.ClassId))
            throw new AdminService.ValidateException("Teacher is not assigned to the selected class.");

        var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Name == scoreDto.SemesterName);
        if (semester == null)
            throw new AdminService.ValidateException("Semester not found. Please enter a valid semester.");

        // New check for academic year matching
        if (semester.AcademicYear != scoreDto.AcademicYear)
            throw new AdminService.ValidateException("Academic year does not match with the semester or semester does not exist.");

        if (scoreDto.Value < 0 || scoreDto.Value > 10)
            throw new ArgumentException("Score must be between 0 and 10.");

        var score = new Score
        {
            StudentId = scoreDto.StudentId,
            SubjectId = scoreDto.SubjectId,
            Value = scoreDto.Value,
            SemesterName = scoreDto.SemesterName,
            ExamType = scoreDto.ExamType,
            AcademicYear = scoreDto.AcademicYear
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
    public double CalculateSemesterAverage(int studentId, int subjectId, string semesterName, ClaimsPrincipal user, string academicYear)
{
    // Lấy email từ claims trong JWT token
    var emailClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    if (emailClaim == null)
        throw new UnauthorizedAccessException("Email claim not found in token.");

    var teacherEmail = emailClaim;

    var teacher = _context.Teachers.FirstOrDefault(t => t.Email == teacherEmail && t.SubjectId == subjectId);
    if (teacher == null)
        throw new UnauthorizedAccessException("Teacher is not assigned to this subject or does not exist.");

    var scores = _context.Scores
        .Where(s => s.StudentId == studentId && s.SubjectId == subjectId && s.SemesterName.Equals(semesterName) && s.AcademicYear == academicYear)
        .ToList();

    if (!scores.Any())
        throw new ArgumentException("No scores found for the specified criteria.");

    double totalScore = 0;
    double countTestWhenClassBegins = 0;
    double countFifteenMinutesTest = 0;

    // Accumulate scores based on exam type
    foreach (var score in scores)
    {
        switch (score.ExamType)
        {
            case "Test when class begins":
                totalScore += score.Value;
                countTestWhenClassBegins += 1;
                break;
            case "15 minutes test":
                totalScore += score.Value;
                countFifteenMinutesTest += 1;
                break;
            case "45 minutes test":
                totalScore += 2 * score.Value;
                break;
            case "semester test":
                totalScore += 3 * score.Value;
                break;
            default:
                throw new ArgumentException("Invalid exam type encountered in score data.");
        }
    }

    var denominator = countTestWhenClassBegins + countFifteenMinutesTest + 5;
    if (denominator == 0)
        throw new InvalidOperationException("Invalid score data. Not enough tests to calculate the average.");

    var average = totalScore / denominator;
    var roundedAverage = Math.Round(average, 1);

    SaveSemesterScore(studentId, subjectId, semesterName, roundedAverage, academicYear);

    return roundedAverage;
}

    public double CalculateAnnualAverage(int studentId, int subjectId, ClaimsPrincipal user, string academicYear)
    {
        var emailClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (emailClaim == null)
            throw new UnauthorizedAccessException("Email claim not found in token.");

        var teacherEmail = emailClaim;

        var teacherSubject = _context.Teachers.FirstOrDefault(t => t.Email == teacherEmail && t.SubjectId == subjectId);
        if (teacherSubject == null)
            throw new UnauthorizedAccessException("Teacher is not assigned to this subject.");

        var semester1Average = CalculateSemesterAverage(studentId, subjectId, "Semester 1", user, academicYear);
        var semester2Average = CalculateSemesterAverage(studentId, subjectId, "Semester 2", user, academicYear);

        var annualAverage = (semester1Average + 2 * semester2Average) / 3;
        var roundedAnnualAverage = Math.Round(annualAverage, 1);

        SaveSemesterScore(studentId, subjectId, "Annual", roundedAnnualAverage, academicYear);

        return roundedAnnualAverage;
    }

    private void SaveSemesterScore(int studentId, int subjectId, string semesterName, double score, string academicYear)
    {
        var studentSubjectScore = _context.StudentSubjectScores
            .FirstOrDefault(sss => sss.StudentId == studentId && sss.SubjectId == subjectId && sss.AcademicYear == academicYear);

        if (studentSubjectScore == null)
        {
            studentSubjectScore = new StudentSubjectScore
            {
                StudentId = studentId,
                SubjectId = subjectId,
                AcademicYear = academicYear
            };
            _context.StudentSubjectScores.Add(studentSubjectScore);
        }

        switch (semesterName.ToLower())
        {
            case "semester 1":
                studentSubjectScore.Semester1Score = score;
                break;
            case "semester 2":
                studentSubjectScore.Semester2Score = score;
                break;
            case "annual":
                studentSubjectScore.AnnualScore = score;
                break;
        }

        _context.SaveChanges();
    }

}