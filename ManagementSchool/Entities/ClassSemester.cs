namespace ManagementSchool.Entities;

public class ClassSemester
{
    public int ClassId { get; set; }
    public Class Class { get; set; }
    public int SemesterId { get; set; }
    public Semester Semester { get; set; }
}