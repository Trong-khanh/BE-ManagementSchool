using Microsoft.EntityFrameworkCore;
using ManagementSchool.Models; // Thay thế bằng namespace chứa ApplicationDbContext của bạn
using ManagementSchool.Entities; // Thay thế bằng namespace chứa các entity của bạn
using ManagementSchool.Dto; // Thay thế bằng namespace chứa các DTO của bạn
using ManagementSchool.Service.TeacherService; // Thay thế bằng namespace chứa ITeacherService

namespace ManagementSchool.Service.TeacherService
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;

        public TeacherService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<SemesterDto>> GetAllSemestersAsync()
        {
            var semesters = await _context.Semesters.Select(s => new SemesterDto
            {
                SemesterId = s.SemesterId,
                Name = s.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            }).ToListAsync();

            return semesters;
        }

        public async Task<bool> AddStudentScoreAsync(ScoreDto scoreDto)
        {
            throw new NotImplementedException();
        }
    }
}