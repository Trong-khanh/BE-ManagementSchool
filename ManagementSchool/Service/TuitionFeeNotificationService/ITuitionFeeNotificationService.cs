using ManagementSchool.Entities;

namespace ManagementSchool.Service.TuitionFeeNotificationService;

public interface ITuitionFeeNotificationService
{
    Task<bool> CreateTuitionFeeNotificationAsync(string semesterType, string academicYear, decimal amount, string content);
    Task<bool> UpdateTuitionFeeNotificationAsync(SemesterType semesterType, string academicYear, decimal amount, string content);
    Task<TuitionFeeNotification> GetTuitionFeeNotificationBySemesterAsync(int semesterId);
    Task<List<TuitionFeeNotification>> GetAllTuitionFeeNotificationsAsync();
} 