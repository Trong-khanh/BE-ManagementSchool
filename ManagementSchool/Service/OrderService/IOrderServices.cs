using ManagementSchool.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagementSchool.Service.OrderService
{
    public interface IOrderServices
    {
        Task SaveOrderAsync(Order order);
        Task<List<Order>> GetAllOrdersAsync(); 
        Task<Order> GetOrderByIdAsync(string orderId); 
    }
}