namespace ManagementSchool.Dto;

public class UpdateClassRequestDto
{
    public string CurrentAcademicYear { get; set; } // Năm học hiện tại
    public string CurrentClassName { get; set; } // Tên lớp hiện tại
    public string NewAcademicYear { get; set; } // Năm học mới
    public string NewClassName { get; set; } // Tên lớp mới
}
