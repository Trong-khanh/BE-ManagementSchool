using ManagementSchool.Entities;

namespace ManagementSchool.Service.StudentService;

public interface IStudentService
{
    IEnumerable<dynamic> GetDailyScores(string studentName, string academicYear);
    IEnumerable<dynamic> GetSubjectsAverageScores(string studentName, string academicYear);
    IEnumerable<dynamic> GetAverageScores(string studentName, string academicYear);
}