using ManagementSchool.Entities;

public class StudentSubjectScore
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
    public double Semester1Score { get; set; }
    public double Semester2Score { get; set; }
    public double? AnnualScore { get; set; }
    public string AcademicYear { get; set; }
}