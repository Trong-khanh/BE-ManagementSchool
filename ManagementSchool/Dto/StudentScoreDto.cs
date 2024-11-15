namespace ManagementSchool.Dto;

public class StudentScoreDto
{
    public string StudentFullName { get; set; }
    public double? SemesterAverage1 { get; set; }
    public double? SemesterAverage2 { get; set; }
    public double? AnnualAverage { get; set; }
    public string AcademicYear { get; set; }
}