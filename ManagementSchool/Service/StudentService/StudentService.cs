using ManagementSchool.Entities;
using ManagementSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementSchool.Service.StudentService;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

        public IEnumerable<dynamic> GetDailyScores(string studentName, string academicYear)
        {
            // Tìm học sinh dựa vào FullName
            var student = _context.Students.FirstOrDefault(s => s.FullName == studentName);
            if (student == null)
            {
                return null;
            }

            // Xác minh AcademicYear trong bảng Semester
            var validSemester = _context.Semesters.FirstOrDefault(s => s.AcademicYear == academicYear);
            if (validSemester == null)
            {
                return null;
            }

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

        public IEnumerable<dynamic> GetSubjectsAverageScores(string studentName, string academicYear)
        {
            var student = _context.Students.FirstOrDefault(s => s.FullName == studentName);
            if (student == null) return null;

            // Kiểm tra nếu academicYear nhập vào null hoặc rỗng, trả về null
            if (string.IsNullOrWhiteSpace(academicYear))
            {
                return null;
            }

            var averageScores = _context.SubjectsAverageScores
                .Where(s => s.StudentId == student.StudentId && s.AcademicYear == academicYear)
                .Include(s => s.Subject)
                .Select(s => new
                {
                    SubjectName = s.Subject.SubjectName,
                    SemesterAverage1 = s.SemesterAverage1.HasValue ? s.SemesterAverage1.ToString() : "Not available",
                    SemesterAverage2 = s.SemesterAverage2.HasValue ? s.SemesterAverage2.ToString() : "Not available",
                    AnnualAverage = s.AnnualAverage.HasValue ? s.AnnualAverage.ToString() : "Not available"
                })
                .ToList();

            return averageScores;
        }

        public IEnumerable<dynamic> GetAverageScores(string studentName, string academicYear)
        {
            var student = _context.Students.FirstOrDefault(s => s.FullName == studentName);
            if (student == null) return null;

            // Kiểm tra nếu academicYear nhập vào null hoặc rỗng, trả về null
            if (string.IsNullOrWhiteSpace(academicYear))
            {
                return null;
            }

            var averageScores = _context.AverageScores
                .Where(s => s.StudentId == student.StudentId && s.AcademicYear == academicYear)
                .Select(s => new
                {
                    AverageSemester1 = s.AverageSemester1.HasValue ? s.AverageSemester1.ToString() : "Not available",
                    AverageSemester2 = s.AverageSemester2.HasValue ? s.AverageSemester2.ToString() : "Not available",
                    AverageAcademicYear = s.AverageAcademicYear.HasValue ? s.AverageAcademicYear.ToString() : "Not available"
                })
                .ToList();

            return averageScores;
        }
}