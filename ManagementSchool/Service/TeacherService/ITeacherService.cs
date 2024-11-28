using System.Security.Claims;
using ManagementSchool.Dto;
using ManagementSchool.Entities;

namespace ManagementSchool.Service.TeacherService;

public interface ITeacherService
{
    Task<IEnumerable<SemesterDto>> GetAllSemestersAsync();
    Task<List<StudentInfoDto>> GetAssignedClassesStudentsAsync(ClaimsPrincipal user);
    Task AddScoreAsync(int studentId, int subjectId, int semesterId, ExamType examType, double scoreValue);
    Task<List<ScoreDto>> GetScoresForStudentAsync(int studentId, int? subjectId = null, int? semesterId = null);
    Task<double> CalculateSemesterAverageAsync(int studentId, int semesterId, ClaimsPrincipal user);
    Task<SemesterAverageScoreDto> GetStudentSubjectAverageScoreAsync(int studentId, int semesterId,
        ClaimsPrincipal user);
}