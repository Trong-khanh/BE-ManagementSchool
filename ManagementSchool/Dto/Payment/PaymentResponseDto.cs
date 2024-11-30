namespace ManagementSchool.Dto;

public class PaymentResponseDto
{
    public bool Success { get; set; }
    public string PayUrl { get; set; }
    public string Message { get; set; }
}