using ManagementSchool.Entities;

public class SemesterDto
{
    public SemesterType SemesterType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string AcademicYear { get; set; }
}