
using System.Security.Claims;
using ManagementSchool.Models;
using ManagementSchool.Entities;
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

        public IEnumerable<dynamic> GetDailyScores(ClaimsPrincipal user, string studentName, string academicYear)
        {
            var student = ResolveParentStudent(user, studentName);
            if (student == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(academicYear))
            {
                return null;
            }

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

        public IEnumerable<dynamic> GetSubjectsAverageScores(ClaimsPrincipal user, string studentName, string academicYear)
        {
            var student = ResolveParentStudent(user, studentName);
            if (student == null) return null;

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

        public IEnumerable<dynamic> GetAverageScores(ClaimsPrincipal user, string studentName, string academicYear)
        {
            var student = ResolveParentStudent(user, studentName);
            if (student == null) return null;

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
                    AverageAcademicYear = s.AverageAcademicYear.HasValue
                        ? s.AverageAcademicYear.ToString()
                        : "Not available"
                })
                .ToList();

            return averageScores;
        }

        private Student? ResolveParentStudent(ClaimsPrincipal user, string studentName)
        {
            var parentEmail = user.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(parentEmail) || string.IsNullOrWhiteSpace(studentName))
            {
                return null;
            }

            return _context.Students.FirstOrDefault(s =>
                s.ParentEmail == parentEmail &&
                s.FullName == studentName);
        }

    }
}
