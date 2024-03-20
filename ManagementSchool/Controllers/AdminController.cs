using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class AdminController: ControllerBase
{
    private readonly StudentService _studentService;

    public AdminController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost("AddStudent")]
    public async Task<IActionResult> AddStudent([FromBody] StudentDtos studentDto)
    {
        try
        {
            var student = await _studentService.AddStudentAsync(studentDto);
            return Ok(student);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}