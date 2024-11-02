using System.Security.Claims;
using ManagementSchool.Dto;

namespace ManagementSchool.Service.TeacherService;

public interface ITeacherService
{
    Task<IEnumerable<SemesterDto>> GetAllSemestersAsync();
    Task<List<StudentInfoDto>> GetAssignedClassesStudentsAsync(ClaimsPrincipal user);
    Task AddScoreForStudentAsync(ClaimsPrincipal user, ScoreDto scoreDto);

    // double CalculateSemesterAverage(int studentId, int subjectId, string semesterName, ClaimsPrincipal user,
    //     string academicYear);
    //
    // double CalculateAnnualAverage(int studentId, int subjectId, ClaimsPrincipal user, string academicYear);
}