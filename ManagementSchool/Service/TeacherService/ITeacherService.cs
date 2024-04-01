using ManagementSchool.Dto;

namespace ManagementSchool.Service.TeacherService;

public interface ITeacherService
{
    Task<IEnumerable<SemesterDto>> GetAllSemestersAsync();
    Task<bool> AddStudentScoreAsync(ScoreDto scoreDto);
}