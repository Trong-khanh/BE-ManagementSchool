using System.Security.Claims;
using ManagementSchool.Dto;

namespace ManagementSchool.Service.ParentService;

public interface IParentService
{
    IEnumerable<dynamic> GetDailyScores(string studentName, string academicYear);
    IEnumerable<dynamic> GetSubjectsAverageScores(string studentName, string academicYear);
    IEnumerable<dynamic> GetAverageScores(string studentName, string academicYear);
}