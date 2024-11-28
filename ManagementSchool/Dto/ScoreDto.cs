using ManagementSchool.Entities;

public class ScoreDto
{
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int SemesterId { get; set; }
    public ExamType ExamType { get; set; }
    public double ScoreValue { get; set; }
}