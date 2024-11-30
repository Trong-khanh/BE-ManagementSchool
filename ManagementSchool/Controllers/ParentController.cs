using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Service.ParentService;
using ManagementSchool.Service.TuitionFeeNotificationService;
using ManagementSchool.Service.MomoService; // Thêm service MomoService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ManagementSchool.Controllers
{
    [Authorize(Roles = "Parent")]
    [Route("api/[controller]")]
    [ApiController]
    public class ParentController : ControllerBase
    {
        private readonly IParentService _parentService;
        private readonly ITuitionFeeNotificationService _tuitionFeeNotificationService;
        private readonly IMomoService _momoService; 
        public ParentController(
            IParentService parentService, 
            ITuitionFeeNotificationService tuitionFeeNotificationService, 
            IMomoService momoService) 
        {
            _parentService = parentService;
            _tuitionFeeNotificationService = tuitionFeeNotificationService;
            _momoService = momoService;
        }

        [HttpGet("GetDailyScores")]
        public IActionResult GetDailyScores([FromQuery] string studentName, [FromQuery] string academicYear)
        {
            var scores = _parentService.GetDailyScores(studentName, academicYear);

            if (scores == null || !scores.Any())
            {
                return NotFound($"No daily scores found for the student '{studentName}' in the academic year '{academicYear}'.");
            }

            return Ok(scores);
        }

        [HttpGet("GetSubjectsAverageScores")]
        public IActionResult GetSubjectsAverageScores([FromQuery] string studentName, [FromQuery] string academicYear)
        {
            var averageScores = _parentService.GetSubjectsAverageScores(studentName, academicYear);

            if (averageScores == null || !averageScores.Any())
            {
                return NotFound($"No subject average scores found for the student '{studentName}' in the academic year '{academicYear}'.");
            }

            return Ok(averageScores);
        }

        [HttpGet("GetAverageScores")]
        public ActionResult<IEnumerable<dynamic>> GetAverageScores(string studentName, string academicYear)
        {
            if (string.IsNullOrWhiteSpace(studentName) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest("Both student name and academic year are required.");
            }

            var averageScores = _parentService.GetAverageScores(studentName, academicYear);

            if (averageScores == null || !averageScores.Any())
            {
                return NotFound($"No average scores found for the student '{studentName}' in the academic year '{academicYear}'.");
            }

            return Ok(averageScores);
        }
        
        // Endpoint to get a tuition fee notification 
        [HttpGet("GetTuitionFeeNotification")]
        public async Task<IActionResult> GetTuitionFeeNotification(SemesterType semesterType, string academicYear)
        {
            var notification = await _tuitionFeeNotificationService.GetTuitionFeeNotificationAsync(semesterType, academicYear);

            if (notification == null)
            {
                return NotFound(new { message = "Tuition fee notification not found." });
            }

            return Ok(notification);
        }

        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto paymentRequest)
        {
            if (paymentRequest == null)
                return BadRequest("Invalid payment request.");

            if (string.IsNullOrEmpty(paymentRequest.SemesterName) || string.IsNullOrEmpty(paymentRequest.AcademicYear))
                return BadRequest("Semester and Academic Year are required.");

            // If orderId is missing, generate it
            if (string.IsNullOrEmpty(paymentRequest.OrderId))
                paymentRequest.OrderId = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            // Retrieve tuition fee notification
            var notification = await _tuitionFeeNotificationService
                .GetTuitionFeeNotificationAsync((SemesterType)Enum.Parse(typeof(SemesterType), paymentRequest.SemesterName), paymentRequest.AcademicYear);

            if (notification == null)
                return NotFound(new { message = "Tuition fee notification not found." });

            // Call MoMo service to create payment
            var response = await _momoService.CreatePaymentAsync(paymentRequest);

            if (response.Success)
            {
                return Ok(new PaymentResponseDto
                {
                    Success = true,
                    PayUrl = response.PayUrl,
                    Message = "Payment created successfully."
                });
            }

            return BadRequest(new PaymentResponseDto
            {
                Success = false,
                Message = response.Message ?? "Failed to create payment."
            });
        }



        // Endpoint to handle MoMo callback
        [HttpGet("PaymentCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback([FromQuery] IQueryCollection query)
        {
            if (query == null || !query.ContainsKey("orderId"))
                return BadRequest("Invalid callback request.");

            var response = await _momoService.PaymentExecuteAsync(query);

            if (response == null)
                return BadRequest("Failed to process payment callback.");

            return Ok(new
            {
                OrderId = response.OrderId,
                Amount = response.Amount,
                FullName = response.FullName,
                OrderInfo = response.OrderInfo
            });
        }
        
    }
}
