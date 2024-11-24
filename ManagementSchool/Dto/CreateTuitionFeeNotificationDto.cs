namespace ManagementSchool.Dto;

public class CreateTuitionFeeNotificationDto
{
    public int SemesterId { get; set; }  
    public decimal Amount { get; set; }  
    public string Content { get; set; }
}