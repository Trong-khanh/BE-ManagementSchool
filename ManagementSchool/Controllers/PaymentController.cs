using ManagementSchool.Entities;
using ManagementSchool.Service.MomoService;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;

public class PaymentController: Controller
{
    private IMomoService _momoService;

    public PaymentController(IMomoService momoService)
    {
        _momoService = momoService;
    }

    [HttpPost ]
    public async Task<IActionResult> CreatePaymentMomo(OrderInfo model)
    {
        var response = await _momoService.CreatePaymentAsync(model);
        return Redirect(response.PayUrl);
    }
    
    [HttpGet]
    public  IActionResult PaymentCallBack()
    {
        var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
        return View(response);
    }
}