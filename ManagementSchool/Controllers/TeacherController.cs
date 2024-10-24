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

    // [HttpGet("ViewAllSemesters")]
    // public async Task<IActionResult> GetAllSemesters()
    // {
    //     try
    //     {
    //         var semesters = await _teacherService.GetAllSemestersAsync();
    //         return Ok(semesters);
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Internal server error: {ex.Message}");
    //     }
    // }

    [HttpPost("AddScore")]
    public async Task<IActionResult> AddScore([FromBody] ScoreDto scoreDto, string teacherEmail)
    {
        try
        {
            await _teacherService.AddScoreAsync(scoreDto, teacherEmail);
            return Ok("Score added successfully.");
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
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


    // [HttpGet("semester-average")]
    // public ActionResult<double> GetSemesterAverage(int studentId, int subjectId, string semesterName,
    //     string academicYear)
    // {
    //     try
    //     {
    //         // Call the service method with the logged-in user's claims and academic year
    //         var average =
    //             _teacherService.CalculateSemesterAverage(studentId, subjectId, semesterName, User, academicYear);
    //         return Ok(average);
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(ex.Message);
    //     }
    // }

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
}