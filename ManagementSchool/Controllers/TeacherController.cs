using ManagementSchool.Dto;
using ManagementSchool.Service;
using ManagementSchool.Service.TeacherService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;
[Authorize(Roles = "Teacher")]
[Route("api/[controller]")]
[ApiController]
public class TeacherController: ControllerBase
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
            return Ok(semesters);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
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
    public async Task<IActionResult> GetStudentsInAssignedClasses(string teacherEmail)
    {
        try
        {
            var students = await _teacherService.GetAssignedClassesStudentsAsync(teacherEmail);
            return Ok(students);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

}