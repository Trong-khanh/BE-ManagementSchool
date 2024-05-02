using ManagementSchool.Service.ParentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;

[Authorize(Roles = "Parent")]
[Route("api/[controller]")]
[ApiController]
public class ParentController : ControllerBase
{
    private readonly IParentService _parentService;

    public ParentController(IParentService parentService)
    {
        _parentService = parentService;
    }

    [HttpGet("GetStudentInfo")]
    public async Task<IActionResult> GetStudentInfo(string className, string studentName, string academicYear)
    {
        try
        {
            var studentScores = await _parentService.GetStudentInfoAsync(className, studentName, academicYear);
            return Ok(studentScores);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}