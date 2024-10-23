using ManagementSchool.Entities;
using ManagementSchool.Models;

namespace ManagementSchool.Dto
{
    public class ScoreDto
    {
        public int StudentId { get; set; }
        public double Value { get; set; }
        public int SemesterId { get; set; } // Renamed to SemesterId
        public ExamType ExamType { get; set; }
        public int ClassId { get; set; }


        public ScoreDto(int studentId, double value, int semesterId, ExamType examType, int classId)
        {
            StudentId = studentId;
            Value = ValidateScore(value);
            SemesterId = semesterId;
            ExamType = examType;
            ClassId = classId;
        }

        private double ValidateScore(double value)
        {
            if (value < 0 || value > 10)
                throw new ArgumentException("Score must be between 0 and 10.");
            return value;
        }

        public static void ValidateSemester(int semesterId, string academicYear, ApplicationDbContext context)
        {
            // Check if the semester exists
            var semester = context.Semesters.Find(semesterId);
            if (semester == null)
                throw new ArgumentException("Semester does not exist.");

            // Check if the academic year matches
            if (semester.AcademicYear != academicYear)
                throw new ArgumentException("Academic year does not match the selected semester.");
        }

        public static void ValidateExamType(string examType)
        {
            if (!Enum.TryParse<ExamType>(examType, true, out var parsedExamType))
            {
                throw new ArgumentException(
                    "Invalid exam type. Please enter a valid type such as 'TestWhenClassBegins', 'FifteenMinutesTest', 'FortyFiveMinutesTest', or 'SemesterTest'.");
            }
        }
    }
}