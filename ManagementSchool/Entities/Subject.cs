namespace ManagementSchool.Entities;

public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public ICollection<Teacher> Teachers { get; set; }
    public ICollection<StudentSubject> StudentSubjects { get; set; }
    public ICollection<ClassSubject> ClassSubjects { get; set; }
    public ICollection<Score> Scores { get; set; }

}