using ManagementSchool.Entities;
using ManagementSchool.Models;
using System.Collections.Generic;
using System.Linq;

namespace ManagementSchool.Dto
{
    public class ScoreDto
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public double Value { get; set; }
        public string SemesterName { get; set; }
        public string ExamType { get; set; }
        public int ClassId { get; set; }
        public string AcademicYear { get; set; } // Trường mới thêm

        private static readonly HashSet<string> ValidExamTypes = new HashSet<string>
        {
            "Test when class begins",
            "15 minutes test",
            "45 minutes test",
            "semester test"
        };

        public void Initialize(int studentId, int subjectId, double value, string semesterName, string examType, int teacherId, string academicYear, ApplicationDbContext context)
        {
            var teacher = context.Teachers.FirstOrDefault(ts => ts.TeacherId == teacherId && ts.SubjectId == subjectId);
            if (teacher == null)
            {
                throw new ArgumentException("Môn học không được giao cho giáo viên này.");
            }

            if (value < 0 || value > 10)
            {
                throw new ArgumentException("Điểm số phải nằm trong khoảng từ 0 đến 10.");
            }

            var semester = context.Semesters.FirstOrDefault(s => s.Name == semesterName);
            if (semester == null)
            {
                throw new ArgumentException("Học kỳ không tồn tại.");
            }

            // Kiểm tra năm học có phù hợp không
            if (semester.AcademicYear != academicYear)
            {
                throw new ArgumentException("Năm học không phù hợp với học kỳ đã chọn.");
            }

            if (!ValidExamTypes.Contains(examType))
            {
                throw new ArgumentException("Loại kỳ thi không hợp lệ. Vui lòng nhập loại hợp lệ như 'Test when class begins', '15 minutes test', '45 minutes test', hoặc 'semester test'.");
            }

            StudentId = studentId;
            SubjectId = subjectId;
            Value = value;
            SemesterName = semesterName;
            ExamType = examType;
            AcademicYear = academicYear; // Thiết lập năm học
        }
    }
}
