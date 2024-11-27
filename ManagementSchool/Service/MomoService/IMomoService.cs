using ManagementSchool.Entities;
using ManagementSchool.Entities.MomoOptonModel;

namespace ManagementSchool.Service.MomoService;

public interface IMomoService
{
    Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model);
    MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
}