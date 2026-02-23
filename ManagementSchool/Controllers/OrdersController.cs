using ManagementSchool.Service.OrderService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
namespace ManagementSchool.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderServices _orderService;

    public OrdersController(IOrderServices orderServices)
    {
        _orderService = orderServices;
    }

    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("GetOrders")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }
    [Authorize(Roles = "Admin,Parent", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("GetOrderById")]
    public async Task<IActionResult> GetOrderById([FromQuery] string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return BadRequest(new { message = "OrderId is required." });

        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
            return NotFound(new { message = "Order not found." });

        return Ok(order);
    }
    [Authorize(Roles = "Admin,Parent", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("getorders/{orderId}")]
    public async Task<IActionResult> GetOrderByIdPath(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return BadRequest(new { message = "OrderId is required." });

        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
            return NotFound(new { message = "Order not found." });

        return Ok(order);
    }

}
