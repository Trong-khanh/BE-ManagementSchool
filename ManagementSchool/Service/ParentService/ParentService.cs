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
        
        public IEnumerable<dynamic> GetDailyScores(string studentName, string className, string academicYear)
        {
            // Tìm ClassId dựa vào ClassName
            var classEntity = _context.Classes.FirstOrDefault(c => c.ClassName == className);
            if (classEntity == null)
            {
                return null; // Không tìm thấy lớp
            }

            int classId = classEntity.ClassId;

            // Tìm học sinh dựa vào ClassId và FullName
            var student = _context.Students.FirstOrDefault(s => s.FullName == studentName && s.ClassId == classId);
            if (student == null)
            {
                return null; // Không tìm thấy học sinh
            }

            // Xác minh AcademicYear trong bảng Semester
            var validSemester = _context.Semesters.FirstOrDefault(s => s.AcademicYear == academicYear);
            if (validSemester == null)
            {
                return null; // Không tìm thấy năm học trong bảng Semester
            }

            // Lấy danh sách điểm của học sinh
            var scores = _context.Scores
                .Where(s => s.StudentId == student.StudentId && s.Semester.AcademicYear == academicYear)
                .Include(s => s.Subject)
                .Include(s => s.Semester)
                .Select(s => new
                {
                    SubjectName = s.Subject.SubjectName,
                    SemesterType = s.Semester.SemesterType,
                    ExamType = s.ExamType,
                    ScoreValue = s.ScoreValue
                })
                .ToList();

            return scores;
        }

    }
}