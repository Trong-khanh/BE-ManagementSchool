namespace ManagementSchool.Entities;

public class Grade
{
    public int GradeId { get; set; }
    public string YearName { get; set; }
    public ICollection<Class> Classes { get; set; }
}