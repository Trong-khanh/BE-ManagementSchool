using ManagementSchool.Dto;
using ManagementSchool.Entities;

namespace ManagementSchool.Service;

public interface IStudentService
{
    Task<Student> AddStudentWithParentAsync(StudentDtos studentDtos);
}