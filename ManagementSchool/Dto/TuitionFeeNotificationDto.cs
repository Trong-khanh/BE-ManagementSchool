namespace ManagementSchool.Dto;

public class TuitionFeeNotificationDto
{
    public string SemesterName { get; set; }
    public string AcademicYear { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string NotificationContent { get; set; }
}