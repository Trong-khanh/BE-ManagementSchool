namespace ManagementSchool.Dto;

public class PaymentRequestDto
{
    public string StudentName { get; set; } 
    public int ClassId { get; set; } 
    public decimal FeeAmount { get; set; }
    public string Description { get; set; }
}