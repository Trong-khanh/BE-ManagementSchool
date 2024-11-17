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

}
