namespace ManagementSchool.Dto;

public class TuitionFeeNotificationDto
{
    public string SemesterName { get; set; }
    public string AcademicYear { get; set; }
    public double Amount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string NotificationContent { get; set; }
}