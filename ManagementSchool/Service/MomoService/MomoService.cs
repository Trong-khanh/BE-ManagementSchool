using System.Security.Cryptography;
using System.Text;
using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Entities.MomoOptonModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
            if (string.IsNullOrWhiteSpace(paymentRequest.OrderId))
            {
                paymentRequest.OrderId = GenerateOrderId();
            }

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
                // Send the request to MoMo API
                var response = await _httpClient.PostAsJsonAsync(_momoOptions.Value.MomoApiUrl, request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response content to your model
                    var responseData = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(responseContent);
                    if (responseData == null)
                    {
                        return new MomoCreatePaymentResponseModel
                        {
                            Success = false,
                            Message = "Invalid response from payment provider.",
                            LocalMessage = "Phản hồi từ cổng thanh toán không hợp lệ.",
                            PayUrl = string.Empty
                        };
                    }

                    // Check if PayUrl exists and assign it, otherwise handle it as required
                    if (!string.IsNullOrEmpty(responseData.PayUrl))
                    {
                        return new MomoCreatePaymentResponseModel
                        {
                            Success = true,
                            Message = responseData.Message,
                            LocalMessage = responseData.LocalMessage,
                            PayUrl = responseData.PayUrl,
                            RequestId = responseData.RequestId,
                            OrderId = responseData.OrderId,
                            Signature = responseData.Signature,
                            QrCodeUrl = responseData.QrCodeUrl,
                            Deeplink = responseData.Deeplink,
                            DeeplinkWebInApp = responseData.DeeplinkWebInApp
                        };
                    }
                    else
                    {
                        return new MomoCreatePaymentResponseModel
                        {
                            Success = false,
                            Message = "No PayUrl found in the response.",
                            LocalMessage = "Không tìm thấy PayUrl trong phản hồi."
                        };
                    }
                }
                else
                {
                    return new MomoCreatePaymentResponseModel
                    {
                        Success = false,
                        Message = "Failed to create payment request.",
                        LocalMessage = "Tạo yêu cầu thanh toán thất bại.",
                        PayUrl = string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                return new MomoCreatePaymentResponseModel
                {
                    Success = false,
                    Message = "Exception occurred: " + ex.Message,
                    PayUrl = string.Empty
                };
            }
        }


        public Task<MomoExecuteResponseModel> PaymentExecuteAsync(IQueryCollection query)
        {
            var orderId = query["orderId"];
            var amount = query["amount"];
            var fullName = query["fullName"];
            var orderInfo = query["orderInfo"];

            return Task.FromResult(new MomoExecuteResponseModel
            {
                OrderId = orderId,
                Amount = amount,
                FullName = fullName,
                OrderInfo = orderInfo
            });
        }

        public bool ValidateCallbackSignature(IQueryCollection query)
        {
            var receivedSignature = query["signature"].ToString();
            if (string.IsNullOrWhiteSpace(receivedSignature))
            {
                return false;
            }

            var rawData = string.Join("&", query
                .Where(kvp => !string.Equals(kvp.Key, "signature", StringComparison.OrdinalIgnoreCase)
                              && !string.Equals(kvp.Key, "signType", StringComparison.OrdinalIgnoreCase))
                .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
                .Select(kvp => $"{kvp.Key}={kvp.Value}"));

            var computedSignature = GenerateSignature(rawData);
            return string.Equals(receivedSignature, computedSignature, StringComparison.OrdinalIgnoreCase);
        }

        private static string GenerateOrderId() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        private string GenerateSignature(string rawData)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_momoOptions.Value.SecretKey));
            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData))).Replace("-", "").ToLower();
        }
    }
}
