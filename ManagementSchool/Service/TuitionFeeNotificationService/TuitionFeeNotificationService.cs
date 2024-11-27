using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementSchool.Service.TuitionFeeNotificationService;

public class TuitionFeeNotificationService : ITuitionFeeNotificationService
{
    private readonly ApplicationDbContext _context;

    public TuitionFeeNotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateTuitionFeeNotificationAsync(string semesterType, string academicYear, decimal amount, string content)
    {
        // Parse semester type from string to enum
        if (!Enum.TryParse<SemesterType>(semesterType.Replace(" ", ""), true, out var semester))
        {
            return false; // Invalid semester type
        }

        // Find the semester in the database
        var semesterFromDb = await _context.Semesters
            .FirstOrDefaultAsync(s => s.SemesterType == semester && s.AcademicYear == academicYear);

        if (semesterFromDb == null) return false; 

        // Check if a tuition fee notification already exists for this semester
        var existingNotification = await _context.TuitionFeeNotifications
            .FirstOrDefaultAsync(n => n.SemesterId == semesterFromDb.SemesterId);
        if (existingNotification != null)
        {
            return false; 
        }

        // Create a new notification
        var notification = new TuitionFeeNotification
        {
            SemesterId = semesterFromDb.SemesterId,
            Amount = amount,
            NotificationContent = content,
            CreatedDate = DateTime.Now,
            IsSent = false // Notification is initially not sent
        };

        _context.TuitionFeeNotifications.Add(notification);
        await _context.SaveChangesAsync();

        return true;
    }
     
    public async Task<bool> UpdateTuitionFeeNotificationAsync(SemesterType semesterType, string academicYear, decimal amount, string content)
    {
        // Tìm kỳ học dựa trên SemesterType và AcademicYear
        var semester = await _context.Semesters
            .FirstOrDefaultAsync(s => s.SemesterType == semesterType && s.AcademicYear == academicYear);

        if (semester == null) return false; // Không tồn tại kỳ học

        // Lấy thông báo học phí đã tồn tại
        var existingNotification = await _context.TuitionFeeNotifications
            .FirstOrDefaultAsync(n => n.SemesterId == semester.SemesterId);

        if (existingNotification == null)
        {
            return false; 
        }

        // Cập nhật thông báo học phí
        existingNotification.Amount = amount;
        existingNotification.NotificationContent = content;
        existingNotification.CreatedDate = DateTime.Now;

        _context.TuitionFeeNotifications.Update(existingNotification);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<TuitionFeeNotificationDto> GetTuitionFeeNotificationAsync(SemesterType semesterType, string academicYear)
    {
        var notification = await _context.TuitionFeeNotifications
            .Include(n => n.Semester)
            .FirstOrDefaultAsync(n => n.Semester.SemesterType == semesterType && n.Semester.AcademicYear == academicYear);

        if (notification == null)
        {
            return null;
        }

        return new TuitionFeeNotificationDto
        {
            SemesterName = notification.Semester.SemesterType.ToString(),
            AcademicYear = notification.Semester.AcademicYear,
            Amount = notification.Amount,
            CreatedDate = notification.CreatedDate,
            NotificationContent = notification.NotificationContent
        };
    }
    
    public async Task<List<TuitionFeeNotification>> GetAllTuitionFeeNotificationsAsync()
    {
        return await _context.TuitionFeeNotifications.ToListAsync();
    }
}