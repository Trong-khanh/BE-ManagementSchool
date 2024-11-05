namespace ManagementSchool.Entities;

public class StudentAverageScore
{
    public int Id { get; set; } // Khóa chính

    // Khóa ngoại đến bảng Student
    public int StudentId { get; set; } 
    public virtual Student Student { get; set; }

    // Khóa ngoại đến bảng Subject
    public int SubjectId { get; set; } 
    public virtual Subject Subject { get; set; }

    // Khóa ngoại đến bảng Semester
    public int SemesterId { get; set; }
    public virtual Semester Semester { get; set; }

    // Năm học
    public string AcademicYear { get; set; }

    // Điểm trung bình cho học kỳ
    public double SemesterAverage { get; set; }

    // Điểm trung bình cả năm
    public double? AnnualAverage { get; set; }
}