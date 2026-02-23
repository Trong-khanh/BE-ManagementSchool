using ManagementSchool.Service.OrderService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
namespace ManagementSchool.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderServices _orderService;

    public OrdersController(IOrderServices orderServices)
    {
        _orderService = orderServices;
    }


    [HttpGet("GetOrders")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("GetOrderById")]
    public async Task<IActionResult> GetOrderById([FromQuery] string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return BadRequest("OrderId is required.");

        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
            return NotFound("Order not found.");

        return Ok(order);
    }

}
