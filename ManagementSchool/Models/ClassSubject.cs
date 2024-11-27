namespace ManagementSchool.Entities;

public class ClassSubject
{
    public int ClassSubjectId { get; set; }
    public int ClassId { get; set; }
    public Class Class { get; set; }

    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
}