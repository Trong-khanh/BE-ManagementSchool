namespace ManagementSchool.Entities;

public class TuitionFeeNotification
{
    public int TuitionFeeNotificationId { get; set; }
    public int SemesterId { get; set; }  
    public Semester Semester { get; set; }

    public decimal Amount { get; set; }  
    public string NotificationContent { get; set; } 
    public DateTime CreatedDate { get; set; } 
    public bool IsSent { get; set; } 
}