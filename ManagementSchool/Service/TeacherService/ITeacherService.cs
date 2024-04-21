using ManagementSchool.Dto;

namespace ManagementSchool.Service.TeacherService;

public interface ITeacherService
{
    Task<IEnumerable<SemesterDto>> GetAllSemestersAsync();
    Task AddScoreAsync(ScoreDto scoreDto, string teacherEmail);
    Task<List<StudentInfoDto>> GetAssignedClassesStudentsAsync(string teacherEmail);
    Task<SemesterScoresDto> CalculateScoreForSemestersAsync(string teacherEmail, int studentId, int subjectId);
    Task<double?> CalculateAnnualAverageScoreAsync(string teacherEmail, int studentId, int subjectId);
}