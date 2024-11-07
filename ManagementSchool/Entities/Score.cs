using ManagementSchool.Entities;

public class Score
{
    public int ScoreId { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int SemesterId { get; set; }

    public double ScoreValue { get; set; }
    public ExamType ExamType { get; set; }

    public virtual Student Student { get; set; }
    public virtual Subject Subject { get; set; }
    public virtual Semester Semester { get; set; }
    public ICollection<SubjectsAverageScore> StudentAverageScores { get; set; }
}