using ManagementSchool.Service.StudentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;

[Authorize(Roles = "Student")]
[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // Endpoint to get the daily scores of a student for a specific academic year
    [HttpGet("GetDailyScores")]
    public IActionResult GetDailyScores([FromQuery] string academicYear)
    {
        var scores = _studentService.GetDailyScores(User, academicYear);

        if (scores == null || !scores.Any())
        {
            return NotFound($"No daily scores found for the current student in the academic year '{academicYear}'.");
        }

        return Ok(scores);
    }

    [HttpGet("GetSubjectsAverageScores")]
    public IActionResult GetSubjectsAverageScores([FromQuery] string academicYear)
    {
        var averageScores = _studentService.GetSubjectsAverageScores(User, academicYear);

        if (averageScores == null || !averageScores.Any())
        {
            return NotFound($"No subject average scores found for the current student in the academic year '{academicYear}'.");
        }

        return Ok(averageScores);
    }

    [HttpGet("GetAverageScores")]
    public ActionResult<IEnumerable<dynamic>> GetAverageScores(string academicYear)
    {
        if (string.IsNullOrWhiteSpace(academicYear))
        {
            return BadRequest("Academic year is required.");
        }

        var averageScores = _studentService.GetAverageScores(User, academicYear);

        if (averageScores == null || !averageScores.Any())
        {
            return NotFound($"No average scores found for the current student in the academic year '{academicYear}'.");
        }

        return Ok(averageScores);
    }
}
