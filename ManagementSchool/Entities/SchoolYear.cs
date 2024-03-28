namespace ManagementSchool.Entities;

public class SchoolYear
{
    public int SchoolYearId { get; set; }
    public string YearName { get; set; }
    public ICollection<Class> Classes { get; set; }
}