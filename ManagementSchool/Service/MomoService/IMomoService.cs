using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Entities.MomoOptonModel;

namespace ManagementSchool.Service.MomoService;

public interface IMomoService
{
    Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(PaymentRequestDto paymentRequest);
    Task<MomoExecuteResponseModel> PaymentExecuteAsync(IQueryCollection query);
}