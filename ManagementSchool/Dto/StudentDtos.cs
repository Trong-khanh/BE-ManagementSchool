using System.ComponentModel.DataAnnotations;

namespace ManagementSchool.Dto;

public class StudentDtos
{
    [Required(ErrorMessage = "FullName is required.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "ClassName is required.")]
    public string ClassName { get; set; }

    [Required(ErrorMessage = "AcademicYear is required.")]
    public string AcademicYear { get; set; }

    [Required(ErrorMessage = "ParentName is required.")]
    public string ParentName { get; set; }

    [Required(ErrorMessage = "ParentEmail is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string ParentEmail { get; set; }
}

