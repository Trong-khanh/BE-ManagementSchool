using ManagementSchool.Entities;

namespace ManagementSchool.Dto;

public class UpdateTuitionFeeNotificationDto
{
    public SemesterType SemesterType { get; set; } 
    public string AcademicYear { get; set; } 
    public double Amount { get; set; }  
    public string Content { get; set; } 
}