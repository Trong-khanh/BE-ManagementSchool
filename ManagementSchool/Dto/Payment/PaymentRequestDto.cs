namespace ManagementSchool.Dto;

public class PaymentRequestDto
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    // public string FullName { get; set; }
    public string CreatedDate { get; set; }
    public string NotificationContent { get; set; }
    public string SemesterName { get; set; }
    public string AcademicYear { get; set; }
}