namespace ManagementSchool.Entities;

public class Score
{
    public int ScoreId { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
    public double Value { get; set; }
    public string SemesterName { get; set; }
    public string ExamType { get; set; }
    public string AcademicYear { get; set; }
}