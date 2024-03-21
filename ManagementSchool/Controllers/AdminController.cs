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
    private readonly IStudentService _studentService;

    public AdminController(IStudentService studentService)
    {
        _studentService = studentService;
    }
    
    [HttpPost("AddStudent")]
    public async Task<IActionResult> AddStudent([FromBody] StudentDtos studentDtos)
    {
        var student = await _studentService.AddStudentWithParentAsync(studentDtos);
        return Ok(student);
    }
    
}