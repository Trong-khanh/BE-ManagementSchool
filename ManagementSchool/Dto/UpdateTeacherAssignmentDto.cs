namespace ManagementSchool.Dto;

public class UpdateTeacherAssignDto
{
    public string TeacherFullName { get; set; }
    public string TeacherEmail { get; set; }
    public string CurrentClassName { get; set; }
    public string NewClassName { get; set; }
}