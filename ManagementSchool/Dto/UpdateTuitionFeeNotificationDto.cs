namespace ManagementSchool.Dto;

public class UpdateTuitionFeeNotificationDto
{
    public int SemesterId { get; set; }  
    public decimal Amount { get; set; }  
    public string Content { get; set; } 
}