using ManagementSchool.Entities;
using ManagementSchool.Models;
using System.Collections.Generic;

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

        // Các ExamTypes hợp lệ
        private static readonly HashSet<string> ValidExamTypes = new HashSet<string>
        {
            "Test when class begins",
            "15 minutes test",
            "45 minutes test",
            "semester test"
        };
        
        public void Initialize(int studentId, int subjectId, double value, string semesterName,
            string examType, int teacherId, ApplicationDbContext context)
        {
            // Kiểm tra môn học có được giao cho giáo viên không
            var isSubjectAssignedToTeacher = context.Teachers.Any
                (ts => ts.TeacherId == teacherId && ts.SubjectId == subjectId);
            if (!isSubjectAssignedToTeacher)
            {
                throw new ArgumentException("The subject is not assigned to the teacher.");
            }

            // Kiểm tra giá trị điểm số có hợp lệ (từ 0 đến 10)
            if (value < 0 || value > 10)
            {
                throw new ArgumentException("Value must be between 0 and 10.");
            }

            // Kiểm tra kỳ học có tồn tại không
            var isSemesterExists = context.Semesters.Any(s => s.Name == semesterName);
            if (!isSemesterExists)
            {
                throw new ArgumentException("Semester does not exist.");
            }

            // Kiểm tra ExamType có hợp lệ không
            if (!ValidExamTypes.Contains(examType))
            {
                throw new ArgumentException("Invalid ExamType entered. Please enter a valid type such as 'Test when class begins', '15 minutes test', 'Check in 45 minutes', or 'semester test'.");
            }
            
            // Thiết lập các thuộc tính nếu tất cả các kiểm tra đều hợp lệ
            StudentId = studentId;
            SubjectId = subjectId;
            Value = value;
            SemesterName = semesterName;
            ExamType = examType;
        }
    }
}
