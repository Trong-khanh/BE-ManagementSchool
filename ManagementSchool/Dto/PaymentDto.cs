namespace ManagementSchool.Dto;

public class PaymentDto
{
    public string StudentName { get; set; } 
    public int ClassId { get; set; } 
    public double Amount { get; set; } 
    public DateTime PaymentDate { get; set; } 
    public string TransactionId { get; set; }
    public bool Success { get; set; } 
}