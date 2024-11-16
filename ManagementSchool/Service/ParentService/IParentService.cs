using System.Security.Claims;
using ManagementSchool.Dto;

namespace ManagementSchool.Service.ParentService;

public interface IParentService
{
    IEnumerable<dynamic> GetDailyScores(string studentName, string className, string academicYear);
}