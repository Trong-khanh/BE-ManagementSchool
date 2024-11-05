using System.Security.Claims;
using ManagementSchool.Dto;
using ManagementSchool.Service;
using ManagementSchool.Service.TeacherService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;

[Authorize(Roles = "Teacher")]
[Route("api/[controller]")]
[ApiController]
public class TeacherController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeacherController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet("ViewAllSemesters")]
    public async Task<IActionResult> GetAllSemesters()
    {
        try
        {
            var semesters = await _teacherService.GetAllSemestersAsync();

            // Check if data exists
            if (semesters == null || !semesters.Any())
            {
                return NotFound("No semesters found.");
            }

            return Ok(semesters);
        }
        catch (Exception ex)
        {
            // Log the exception if logging is set up (optional)
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpGet("GetStudentsInAssignedClasses")]
    public async Task<IActionResult> GetStudentsInAssignedClasses()
    {
        try
        {
            // Gọi dịch vụ để lấy danh sách học sinh
            var students = await _teacherService.GetAssignedClassesStudentsAsync(User);
            return Ok(students);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    [HttpPost("AddScore")]
    public async Task<IActionResult> AddScoreForStudent([FromBody] ScoreDto scoreDto)
    {
        try
        {
            await _teacherService.AddScoreForStudentAsync(User, scoreDto);
            return Ok(new { Message = "Score added successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet(" GetScoreStudent/{studentId}")]
    public async Task<IActionResult> GetScoresForStudent(int studentId, [FromQuery] int? subjectId = null,
        [FromQuery] int? semesterId = null)

    {
        try
        {
            var scores = await _teacherService.GetScoresForStudentAsync(studentId, subjectId, semesterId);
            if (scores == null || scores.Count == 0)
            {
                return NotFound("Không tìm thấy điểm cho sinh viên.");
            }

            return Ok(scores);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Lỗi khi tải dữ liệu: {ex.Message}" });
        }
    }

    [HttpGet("CalculateSemesterAverage")]
    public async Task<IActionResult> CalculateSemesterAverage(
        int studentId,
        int semesterId)
    {
        // Lấy user đã xác thực
        ClaimsPrincipal user = HttpContext.User;

        try
        {
            // Gọi phương thức service để tính điểm trung bình học kỳ
            double average = await _teacherService.CalculateSemesterAverageAsync(studentId, semesterId, user);

            // Trả về điểm trung bình đã tính toán
            return Ok(new { SemesterAverage = average });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            // Xử lý bất kỳ ngoại lệ nào khác có thể xảy ra
            return StatusCode(500,
                new { Message = "An error occurred while calculating the semester average.", Details = ex.Message });
        }
    }
}

// [HttpGet("annual-average")]
// public ActionResult<double> GetAnnualAverage(int studentId, int subjectId, string academicYear)
// {
//     try
//     {
//         // Call the service method with the logged-in user's claims and academic year
//         var average = _teacherService.CalculateAnnualAverage(studentId, subjectId, User, academicYear);
//         return Ok(average);
//     }
//     catch (Exception ex)
//     {
//         return BadRequest(ex.Message);
//     }
// }