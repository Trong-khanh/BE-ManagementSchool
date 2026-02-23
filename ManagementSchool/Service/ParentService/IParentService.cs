using System.Security.Claims;

namespace ManagementSchool.Service.ParentService;

public interface IParentService
{
    IEnumerable<dynamic> GetDailyScores(ClaimsPrincipal user, string studentName, string academicYear);
    IEnumerable<dynamic> GetSubjectsAverageScores(ClaimsPrincipal user, string studentName, string academicYear);
    IEnumerable<dynamic> GetAverageScores(ClaimsPrincipal user, string studentName, string academicYear);
}
