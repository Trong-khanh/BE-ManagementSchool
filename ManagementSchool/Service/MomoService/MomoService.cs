using System.Security.Cryptography;
using System.Text;
using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Entities.MomoOptonModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace ManagementSchool.Service.MomoService
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _momoOptions;
    private readonly HttpClient _httpClient;

    public MomoService(IOptions<MomoOptionModel> momoOptions, HttpClient httpClient)
    {
        _momoOptions = momoOptions;
        _httpClient = httpClient;
    }

    public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(PaymentRequestDto paymentRequest)
    {
        paymentRequest.OrderId = GenerateOrderId();
        var requestId = Guid.NewGuid().ToString();
        var rawData = $"partnerCode={_momoOptions.Value.PartnerCode}" +
                      $"&accessKey={_momoOptions.Value.AccessKey}" +
                      $"&requestId={requestId}" +
                      $"&amount={paymentRequest.Amount.ToString()}" +
                      $"&orderId={paymentRequest.OrderId}" +
                      $"&orderInfo={paymentRequest.NotificationContent}" +
                      $"&returnUrl={_momoOptions.Value.ReturnUrl}" +
                      $"&notifyUrl={_momoOptions.Value.NotifyUrl}" +
                      $"&extraData=";

        
        var signature = GenerateSignature(rawData);
        Console.WriteLine($"Raw Data: {rawData}");


        var request = new
        {
            partnerCode = _momoOptions.Value.PartnerCode,
            accessKey = _momoOptions.Value.AccessKey,
            requestId = requestId,
            orderId = paymentRequest.OrderId,
            amount = paymentRequest.Amount.ToString(),
            orderInfo = paymentRequest.NotificationContent,
            returnUrl = _momoOptions.Value.ReturnUrl,
            notifyUrl = _momoOptions.Value.NotifyUrl,
            requestType = _momoOptions.Value.RequestType,
            signature = signature
        };
        
        try
        {
            
            var response = await _httpClient.PostAsJsonAsync(_momoOptions.Value.MomoApiUrl, request);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<MomoCreatePaymentResponseModel>();
                return responseData;
            }

            return new MomoCreatePaymentResponseModel
            {
                Success = false,
                Message = await response.Content.ReadAsStringAsync()
            };
        }
        catch (Exception ex)
        {
            return new MomoCreatePaymentResponseModel
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<MomoExecuteResponseModel> PaymentExecuteAsync(IQueryCollection query)
    {
        var orderId = query["orderId"];
        var amount = query["amount"];
        var fullName = query["fullName"];
        var orderInfo = query["orderInfo"];

        return new MomoExecuteResponseModel
        {
            OrderId = orderId,
            Amount = amount,
            FullName = fullName,
            OrderInfo = orderInfo
        };
    }

    private string GenerateOrderId() => DateTime.UtcNow.Ticks.ToString();

    private string GenerateSignature(string rawData)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_momoOptions.Value.SecretKey));
        return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData))).Replace("-", "").ToLower();
    }
    
    }
}