using ManagementSchool.Dto;
using ManagementSchool.Entities;

namespace ManagementSchool.Service;

public interface IStudentService
{
    Task<Student> AddStudentWithParentAsync(StudentDtos studentDtos);
    Task<bool> DeleteStudentAsync(int studentId);
    Task<Student> UpdateStudentAsync(int studentId, StudentDtos studentDtos);
    Task<IEnumerable<Student>> GetStudentsByClassAsync(string className);
    Task<IEnumerable<Student>> GetStudentsBySchoolYearAsync(string YearName);
    Task<Student> GetStudentByIdAsync(int studentId);

}