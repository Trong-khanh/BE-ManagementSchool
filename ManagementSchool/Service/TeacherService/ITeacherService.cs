using System.Security.Claims;
using ManagementSchool.Dto;

namespace ManagementSchool.Service.TeacherService;

public interface ITeacherService
{
    Task<IEnumerable<SemesterDto>> GetAllSemestersAsync();
    Task<List<StudentInfoDto>> GetAssignedClassesStudentsAsync(ClaimsPrincipal user);
    Task AddScoreForStudentAsync(ClaimsPrincipal user, ScoreDto scoreDto);
    Task<List<ScoreDto>> GetScoresForStudentAsync(int studentId, int? subjectId = null, int? semesterId = null);
    Task<double> CalculateSemesterAverageAsync(int studentId, int semesterId, ClaimsPrincipal user);

    //
    // double CalculateAnnualAverage(int studentId, int subjectId, ClaimsPrincipal user, string academicYear);
}