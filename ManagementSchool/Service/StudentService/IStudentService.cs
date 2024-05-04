using ManagementSchool.Entities;

namespace ManagementSchool.Service.StudentService;

public interface IStudentService
{
    Task<Student> GetStudentByIdAsync(int studentId);
}