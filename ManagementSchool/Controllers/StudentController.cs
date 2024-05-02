using ManagementSchool.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly IAdminService _adminService;

    public StudentController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [Authorize(Roles = "Student")]
    [HttpGet("GetStudentInfo/{studentId}")]
    public async Task<IActionResult> GetStudentInfo(int studentId)
    {
        try
        {
            var student = await _adminService.GetStudentByIdAsync(studentId);
            if (student == null)
                return NotFound("Student not found.");

            return Ok(new
            {
                student.FullName,
                student.Class.ClassName,
                student.Address,
                student.Parent.ParentName
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}