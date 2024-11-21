namespace ManagementSchool.Dto;

public class FeeNotificationDto
{
    public int ClassId { get; set; } 
    public double FeeAmount { get; set; } 
    public string Description { get; set; } 
    public DateTime DueDate { get; set; } 
}