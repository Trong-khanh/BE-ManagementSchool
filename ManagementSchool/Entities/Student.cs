namespace ManagementSchool.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public int ClassId { get; set; }
    public Class Class { get; set; }
    public int ParentId { get; set; }
    public Parent Parent { get; set; }
}