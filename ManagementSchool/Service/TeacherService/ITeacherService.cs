using ManagementSchool.Dto;
using ManagementSchool.Entities;

namespace ManagementSchool.Service.TeacherService;

public interface ITeacherService
{
    Task<TeacherDto?> AddTeacherAsync(TeacherDto teacherDto);
    Task<bool> DeleteTeacherAsync(int teacherId);
    Task<TeacherDto?> UpdateTeacherAsync(int teacherId, TeacherDto teacherDto);
    Task<IEnumerable<TeacherDto>> GetAllTeachersAsync();
    Task<bool> DeleteTeacherByNameAsync( string teacherName);
    Task<IEnumerable<TeacherDto>> GetTeachersBySubjectAsync(string subjectName);
}