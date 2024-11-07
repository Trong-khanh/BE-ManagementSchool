using ManagementSchool.Entities;

public class SemesterDto
{
    public int SemesterId { get; set; }
    public string SemesterType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string AcademicYear { get; set; }
}