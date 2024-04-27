namespace ManagementSchool.Entities;

public class Semester
{
    public int SemesterId { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; } 
    public DateTime EndDate { get; set; }
    public string AcademicYear { get; set; } 
    public ICollection<ClassSemester> ClassSemesters { get; set; }
}