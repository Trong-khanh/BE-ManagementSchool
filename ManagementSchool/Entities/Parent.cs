namespace ManagementSchool.Entities;

public class Parent
{
    public int ParentId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public ICollection<Student> Students { get; set; }
}