namespace ManagementSchool.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentStatus { get; set; } 
    public string PayPalTransactionId { get; set; }
}