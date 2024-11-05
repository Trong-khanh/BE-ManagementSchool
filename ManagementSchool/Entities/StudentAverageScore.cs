namespace ManagementSchool.Entities;

public class StudentAverageScore
{
    public int Id { get; set; } 

    public int StudentId { get; set; } 
    public virtual Student Student { get; set; }
    public int SubjectId { get; set; } 
    public virtual Subject Subject { get; set; }
    public int SemesterId { get; set; }
    public virtual Semester Semester { get; set; }
    public string AcademicYear { get; set; }

    public double? SemesterAverage1 { get; set; }
    public double? SemesterAverage2 { get; set; }

    public double? AnnualAverage { get; set; }
}