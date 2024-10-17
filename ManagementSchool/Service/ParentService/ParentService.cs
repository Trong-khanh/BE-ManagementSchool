using System.Linq;
using System.Threading.Tasks;
using ManagementSchool.Models;
using ManagementSchool.Dto; // Thêm namespace để sử dụng DTOs
using Microsoft.EntityFrameworkCore;

namespace ManagementSchool.Service.ParentService
{
    public class ParentService : IParentService
    {
        private readonly ApplicationDbContext _context;

        public ParentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentScoreInfoDto>> GetStudentInfoAsync(string className, string studentName,
            string academicYear)
        {
            var studentScores = await _context.Students
                .Where(s => s.FullName == studentName &&
                            s.Class.ClassName == className &&
                            s.AcademicYear == academicYear)
                .SelectMany(s => s.Scores, (s, score) => new StudentScoreInfoDto
                {
                    StudentFullName = s.FullName,
                    ClassName = s.Class.ClassName,
                    SubjectName = score.Subject.SubjectName,
                    ScoreValue = score.Value,
                    ExamType = score.ExamType,
                    Semester = score.SemesterName,
                    AcademicYear = s.AcademicYear
                })
                .ToListAsync();

            if (!studentScores.Any())
            {
                throw new Exception("No scores found for the provided student information.");
            }

            return studentScores;
        }
    }
}