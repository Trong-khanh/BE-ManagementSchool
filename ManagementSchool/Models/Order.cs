namespace ManagementSchool.Entities;

public class Order
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public string SemesterName { get; set; }
    public string AcademicYear { get; set; }
    public string NotificationContent { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime CreatedDate { get; set; }
}