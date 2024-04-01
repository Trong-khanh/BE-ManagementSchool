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

}