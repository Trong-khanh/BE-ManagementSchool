using ManagementSchool.Dto;

namespace ManagementSchool.Service.TeacherService;

public interface ITeacherService
{
    Task<IEnumerable<SemesterDto>> GetAllSemestersAsync();
    Task AddScoreAsync(ScoreDto scoreDto, string teacherEmail);
    Task<List<StudentInfoDto>> GetAssignedClassesStudentsAsync(string teacherEmail);
    double CalculateSemesterAverage(int studentId, int subjectId, string semesterName);
    double CalculateAnnualAverage(int studentId, int subjectId);
}