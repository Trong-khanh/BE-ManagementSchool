using ManagementSchool.Dto;
using ManagementSchool.Entities;

namespace ManagementSchool.Service;

public interface IAdminService
{
    Task<Student> AddStudentWithParentAsync(StudentDtos studentDtos);
    Task<bool> DeleteStudentAsync(int studentId);
    Task<Student> UpdateStudentAsync(int studentId, StudentDtos studentDtos);
    Task<TeacherDto?> AddTeacherAsync(TeacherDto teacherDto);
    Task<bool> DeleteTeacherAsync(int teacherId);
    Task<TeacherDto?> UpdateTeacherAsync(int teacherId, TeacherDto teacherDto);
    Task<IEnumerable<TeacherWithSubjectDto>> GetAllTeachersAsync();
    Task AssignTeacherToClassAsync(TeacherClassAssignDto assignDto);
    Task<List<TeacherClassAssignDto>> GetTeacherClassAssignedAsync();
    Task UpdateTeacherClassAssignmentAsync(UpdateTeacherAssignDto assignDto);
    Task DeleteTeacherFromClassAsync(TeacherClassAssignDto assignDto);
    Task<Semester> AddSemesterAsync(SemesterDto semesterDto);
    Task<Semester> UpdateSemesterAsync(int semesterId, SemesterDto semesterDto);
    Task<bool> DeleteSemesterAsync(int semesterId);
    Task<IEnumerable<SemesterDto>> GetAllSemestersAsync();
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<bool> UpgradeClassAsync(int oldClassId, string oldAcademicYear, int newClassId, string newAcademicYear);
    Task<Class> AddClassAsync(ClassDto newClassDto);
    Task<Class> UpdateClassAsync(int classId, ClassDto updatedClassDto);
    Task<bool> DeleteClassAsync(int classId);
    Task<IEnumerable<Class>> GetAllClassesAsync();
    Task<Class> GetClassByIdAsync(int classId);
    Task<List<AverageScore>> CalculateAndSaveAverageScoresForClassAsync(string className, string academicYear);
    Task<IEnumerable<StudentScoreDto>> GetStudentAverageScoresAsync(int classId, string academicYear);
    Task UpdateClassAndResetScoresAsync(string currentAcademicYear, string currentClassName, string newAcademicYear,
        string newClassName);
}