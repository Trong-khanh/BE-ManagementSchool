namespace ManagementSchool.Dto
{
    public class TeacherDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int? SubjectId { get; set; } 
        public string? SubjectName { get; set; }
    }
}