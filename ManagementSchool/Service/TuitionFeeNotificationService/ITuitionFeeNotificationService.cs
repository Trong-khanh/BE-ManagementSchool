using ManagementSchool.Entities;

namespace ManagementSchool.Service.TuitionFeeNotificationService;

public interface ITuitionFeeNotificationService
{
    Task<bool> CreateTuitionFeeNotificationAsync(int semesterId, decimal amount, string content);
    Task<bool> UpdateTuitionFeeNotificationAsync(int semesterId, decimal amount, string content);
    Task<TuitionFeeNotification> GetTuitionFeeNotificationBySemesterAsync(int semesterId);
    Task<List<TuitionFeeNotification>> GetAllTuitionFeeNotificationsAsync();
}