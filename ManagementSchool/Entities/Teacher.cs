namespace ManagementSchool.Entities;

public class Teacher
{
    public int TeacherId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<TeacherClass> TeacherClasses { get; set; }
}