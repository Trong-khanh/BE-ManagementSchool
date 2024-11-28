using ManagementSchool.Dto;
using ManagementSchool.Entities;

namespace ManagementSchool.Service.TuitionFeeNotificationService;

public interface ITuitionFeeNotificationService
{
    Task<bool> CreateTuitionFeeNotificationAsync(string semesterType, string academicYear, decimal amount, string content);
    Task<bool> UpdateTuitionFeeNotificationAsync(SemesterType semesterType, string academicYear, decimal amount, string content);
    Task<TuitionFeeNotificationDto> GetTuitionFeeNotificationAsync(SemesterType semesterType, string academicYear);
} 