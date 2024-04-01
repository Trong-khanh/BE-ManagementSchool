namespace ManagementSchool.Dto;

public class ScoreDto
{
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public double Value { get; set; } // Giá trị điểm số
    public string SemesterName { get; set; } // Tên học kỳ
}