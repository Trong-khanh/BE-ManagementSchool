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

    public async Task<bool> CreateTuitionFeeNotificationAsync(int semesterId, decimal amount, string content)
    {
        // Kiểm tra xem kỳ học có tồn tại không
        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null) return false; // Không tồn tại kỳ học

        // Kiểm tra nếu đã có thông báo học phí
        var existingNotification = await _context.TuitionFeeNotifications
            .FirstOrDefaultAsync(n => n.SemesterId == semesterId);
        if (existingNotification != null)
        {
            return false; // Đã có thông báo học phí cho kỳ học này
        }

        // Tạo thông báo học phí mới
        var notification = new TuitionFeeNotification
        {
            SemesterId = semesterId,
            Amount = amount,
            NotificationContent = content,
            CreatedDate = DateTime.Now,
            IsSent = false // Trạng thái ban đầu chưa gửi
        };

        _context.TuitionFeeNotifications.Add(notification);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateTuitionFeeNotificationAsync(int semesterId, decimal amount, string content)
    {
        // Kiểm tra xem kỳ học có tồn tại không
        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null) return false;

        // Lấy thông báo học phí đã tồn tại
        var existingNotification = await _context.TuitionFeeNotifications
            .FirstOrDefaultAsync(n => n.SemesterId == semesterId);

        if (existingNotification == null)
        {
            return false; // Không tìm thấy thông báo học phí để cập nhật
        }

        // Cập nhật thông báo học phí
        existingNotification.Amount = amount;
        existingNotification.NotificationContent = content;
        existingNotification.CreatedDate = DateTime.Now;

        _context.TuitionFeeNotifications.Update(existingNotification);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TuitionFeeNotification> GetTuitionFeeNotificationBySemesterAsync(int semesterId)
    {
        return await _context.TuitionFeeNotifications
            .FirstOrDefaultAsync(n => n.SemesterId == semesterId);
    }
    
    public async Task<List<TuitionFeeNotification>> GetAllTuitionFeeNotificationsAsync()
    {
        return await _context.TuitionFeeNotifications.ToListAsync();
    }

}