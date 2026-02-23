using ManagementSchool.Entities;
using ManagementSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementSchool.Service.OrderService
{
    public class OrderServices : IOrderServices
    {
        private readonly ApplicationDbContext _context;

        public OrderServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveOrderAsync(Order order)
        {
            var existingOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            if (existingOrder == null)
            {
                await _context.Orders.AddAsync(order);
            }
            else if (!ReferenceEquals(existingOrder, order))
            {
                existingOrder.Amount = order.Amount;
                existingOrder.SemesterName = order.SemesterName;
                existingOrder.AcademicYear = order.AcademicYear;
                existingOrder.NotificationContent = order.NotificationContent;
                existingOrder.PaymentStatus = order.PaymentStatus;
                existingOrder.CreatedDate = order.CreatedDate;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
