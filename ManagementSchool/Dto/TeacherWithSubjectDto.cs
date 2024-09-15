namespace ManagementSchool.Dto;

public class TeacherWithSubjectDto
{
    public int TeacherId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
}