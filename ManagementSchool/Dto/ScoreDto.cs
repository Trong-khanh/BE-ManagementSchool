using ManagementSchool.Entities;

public class ScoreDto
{
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int SemesterId { get; set; }
    public double ScoreValue { get; set; }
    public string ExamType { get; set; }
}
