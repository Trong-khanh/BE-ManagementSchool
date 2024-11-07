namespace ManagementSchool.Entities;

public class AverageScore
{
    public int AverageScoreId { get; set; }
    public double? AverageSemester1 { get; set; }
    public double? AverageSemester2 { get; set; }
    public double? AverageAcademicYear { get; set; }
    
    public int StudentId { get; set; }
    public Student Student { get; set; }
}