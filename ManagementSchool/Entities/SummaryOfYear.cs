namespace ManagementSchool.Entities;

public class SummaryOfYear
{
    public int id { get; set; }
    public int FinalGrade { get; set; }
    public string Classification { get; set; }
    public string Status { get; set; }
    public int StudentId { get; set; }  
    public Student Student { get; set; }
    public string AcademicYear { get; set; }
}