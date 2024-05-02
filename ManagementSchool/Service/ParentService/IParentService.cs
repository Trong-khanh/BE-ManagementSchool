using System.Security.Claims;
using ManagementSchool.Dto;

namespace ManagementSchool.Service.ParentService;

public interface IParentService
{
    Task<List<StudentScoreInfoDto>> GetStudentInfoAsync(string className, string studentName, string academicYear);
}