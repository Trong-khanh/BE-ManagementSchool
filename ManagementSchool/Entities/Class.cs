using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementSchool.Entities;

public class Class
{
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public int SchoolYearId { get; set; }
    public SchoolYear SchoolYear { get; set; }
    public ICollection<Student> Students { get; set; }
    public ICollection<TeacherClass> TeacherClasses { get; set; }
    public ICollection<ClassSubject> ClassSubjects { get; set; }
    public ICollection<ClassSemester> ClassSemesters { get; set; }
}