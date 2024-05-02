namespace ManagementSchool.Entities;

public class Parent
{
    public int ParentId { get; set; }
    public string ParentName { get; set; }
    public ICollection<Student> Students { get; set; }
}