using ManagementSchool.Dto;
using ManagementSchool.Entities;

namespace ManagementSchool.Service;

public interface IAdminService
{
    Task<Student> AddStudentWithParentAsync(StudentDtos studentDtos);
    Task<bool> DeleteStudentAsync(int studentId);
    Task<Student> UpdateStudentAsync(int studentId, StudentDtos studentDtos);
    Task<IEnumerable<Student>> GetStudentsByClassAsync(string className);
    Task<IEnumerable<Student>> GetStudentsBySchoolYearAsync(string YearName);
    Task<Student> GetStudentByIdAsync(int studentId);
    Task<TeacherDto?> AddTeacherAsync(TeacherDto teacherDto);
    Task<bool> DeleteTeacherAsync(int teacherId);
    Task<TeacherDto?> UpdateTeacherAsync(int teacherId, TeacherDto teacherDto);
    Task<IEnumerable<TeacherDto>> GetAllTeachersAsync();
    Task<bool> DeleteTeacherByNameAsync(string teacherName);
    Task<IEnumerable<TeacherDto>> GetTeachersBySubjectAsync(string subjectName);
    Task AssignTeacherToClassAsync(TeacherClassAssignmentDto assignmentDto);
    Task<List<TeacherClassAssignmentDto>> GetTeacherClassAssignmentsAsync();
    Task<Semester> AddSemesterAsync(SemesterDto semesterDto);
    Task<Semester> UpdateSemesterAsync(int semesterId, SemesterDto semesterDto);
    Task<bool> DeleteSemesterAsync(int semesterId);
    Task<IEnumerable<Semester>> GetAllSemestersAsync();
    Task<Semester> GetSemesterByIdAsync(int semesterId);
}