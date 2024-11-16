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
    [HttpGet("GetsSoresDaily")]
    public IActionResult GetDailyScores([FromQuery] string studentName, [FromQuery] string className, [FromQuery] string academicYear)
    {
        var scores = _parentService.GetDailyScores(studentName, className, academicYear);

        if (scores == null || !scores.Any())
        {
            return NotFound("Student not found .");
        }

        return Ok(scores);
    }
}