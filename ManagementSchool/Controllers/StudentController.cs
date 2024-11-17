using ManagementSchool.Service.StudentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;

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
    [Authorize(Roles = "Student")] // Restrict access to students
    [HttpGet("GetDailyScores")]
    public IActionResult GetDailyScores([FromQuery] string studentName, [FromQuery] string academicYear)
    {
        // Retrieve the daily scores of the student for the specified academic year
        var scores = _studentService.GetDailyScores(studentName, academicYear);

        // If no scores are found, return a NotFound response with a descriptive message
        if (scores == null || !scores.Any())
        {
            return NotFound($"No daily scores found for the student '{studentName}' in the academic year '{academicYear}'.");
        }

        // Return the daily scores in JSON format
        return Ok(scores);
    }

    // Endpoint to get the average scores for each subject of a student
    [HttpGet("GetSubjectsAverageScores")]
    public IActionResult GetSubjectsAverageScores([FromQuery] string studentName, [FromQuery] string academicYear)
    {
        // Retrieve the average scores for each subject of the student for the specified academic year
        var averageScores = _studentService.GetSubjectsAverageScores(studentName, academicYear);

        // If no average scores are found, return a NotFound response with a descriptive message
        if (averageScores == null || !averageScores.Any())
        {
            return NotFound($"No subject average scores found for the student '{studentName}' in the academic year '{academicYear}'.");
        }

        // Return the average scores for each subject in JSON format
        return Ok(averageScores);
    }

    // Endpoint to get the overall average score for a student
    [HttpGet("GetAverageScores")]
    public ActionResult<IEnumerable<dynamic>> GetAverageScores(string studentName, string academicYear)
    {
        // Check if the required query parameters are provided
        if (string.IsNullOrWhiteSpace(studentName) || string.IsNullOrWhiteSpace(academicYear))
        {
            return BadRequest("Both student name and academic year are required."); // Return 400 if any parameter is missing
        }

        // Retrieve the overall average scores of the student for the specified academic year
        var averageScores = _studentService.GetAverageScores(studentName, academicYear);

        // If no average scores are found, return a NotFound response with a descriptive message
        if (averageScores == null || !averageScores.Any())
        {
            return NotFound($"No average scores found for the student '{studentName}' in the academic year '{academicYear}'.");
        }

        // Return the average scores in JSON format
        return Ok(averageScores);
    }
}
