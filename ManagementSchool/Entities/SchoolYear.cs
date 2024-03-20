namespace ManagementSchool.Entities;

public class SchoolYear
{
    public int SchoolYearId { get; set; }
    public string Name { get; set; }
    public ICollection<Class> Classes { get; set; }
    
}