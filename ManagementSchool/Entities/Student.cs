using System.Text.Json.Serialization;

namespace ManagementSchool.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public int ClassId { get; set; }
    public Class Class { get; set; }
    public int ParentId { get; set; }
    public Parent Parent { get; set; }
    public ICollection<Subject> Subjects { get; set; }
    public ICollection<StudentSubject> StudentSubjects { get; set; }
    public ICollection<ClassSemester> ClassSemesters { get; set; }
}