using ManagementSchool.Entities;
using System.Security.Claims;

namespace ManagementSchool.Service.StudentService;

public interface IStudentService
{
    IEnumerable<dynamic> GetDailyScores(ClaimsPrincipal user, string academicYear);
    IEnumerable<dynamic> GetSubjectsAverageScores(ClaimsPrincipal user, string academicYear);
    IEnumerable<dynamic> GetAverageScores(ClaimsPrincipal user, string academicYear);
}
