using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
        
    }

    // START CRUD STUDENT 

    // Add a new student along with the parent details.
    [HttpPost("AddStudent")]
    public async Task<IActionResult> AddStudent([FromBody] StudentDtos studentDtos)
    {
        var student = await _adminService.AddStudentWithParentAsync(studentDtos);
        return Ok(student);
    }

    // Delete a student by their ID.
    [HttpDelete("DeleteStudent/{studentId}")]
    public async Task<IActionResult> DeleteStudent(int studentId)
    {
        var result = await _adminService.DeleteStudentAsync(studentId);
        if (!result) return NotFound("Student not found.");
        return Ok("Student deleted successfully.");
    }

    // Update a student's details.
    [HttpPut("UpdateStudent/{studentId}")]
    public async Task<IActionResult> UpdateStudent(int studentId, [FromBody] StudentDtos studentDtos)
    {
        try
        {
            var updatedStudent = await _adminService.UpdateStudentAsync(studentId, studentDtos);
            return Ok(updatedStudent);
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest($"Validation Error: {ex.Message}");
        }
    }

    // Get students by class and academic year.
    [HttpGet("GetStudentsByClass/{className}/{academicYear}")]
    public async Task<IActionResult> GetStudentsByClass(string className, string academicYear)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(className))
                return BadRequest("Class name cannot be empty.");

            var students = await _adminService.GetStudentsByClassAsync(className, academicYear);
            return Ok(students);
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Get all students in the system.
    [HttpGet("getaAllStudents")]
    public async Task<IActionResult> GetAllStudents()
    {
        return Ok(await _adminService.GetAllStudentsAsync());
    }

    // END CRUD STUDENT 

    //====================================================================================================
    // START CRUD TEACHER

    // Add a new teacher to the system.
    [HttpPost("AddTeacher")]
    public async Task<IActionResult> AddTeacher([FromBody] TeacherDto teacherDto)
    {
        try
        {
            var newTeacher = await _adminService.AddTeacherAsync(teacherDto);
            return Ok(newTeacher);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Delete a teacher by their ID.
    [HttpDelete("DeleteTeacher/{teacherId}")]
    public async Task<IActionResult> DeleteTeacher(int teacherId)
    {
        try
        {
            var result = await _adminService.DeleteTeacherAsync(teacherId);
            if (!result) return NotFound("Teacher not found.");
            return Ok("Teacher deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Update teacher details.
    [HttpPut("UpdateTeacher/{teacherId}")]
    public async Task<IActionResult> UpdateTeacher(int teacherId, [FromBody] TeacherDto teacherDto)
    {
        try
        {
            var updatedTeacher = await _adminService.UpdateTeacherAsync(teacherId, teacherDto);
            return Ok(updatedTeacher);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Get all teachers in the system.
    [HttpGet("GetAllTeachers")]
    public async Task<IActionResult> GetAllTeachers()
    {
        try
        {
            var teachers = await _adminService.GetAllTeachersAsync();
            return Ok(teachers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // END CRUD TEACHER

    // Assign a teacher to a class.
    [HttpPost("AssignTeacherToClass")]
    public async Task<IActionResult> AssignTeacherToClass([FromBody] TeacherClassAssignDto assigntDto)
    {
        try
        {
            await _adminService.AssignTeacherToClassAsync(assigntDto);
            return Ok("Teacher successfully assigned to the class.");
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest($"Validation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Get all teacher-class assignments.
    [HttpGet("GetTeacherClassAssigned")]
    public async Task<IActionResult> GetTeacherClassAssignments()
    {
        try
        {
            var assignments = await _adminService.GetTeacherClassAssignedAsync();
            return Ok(assignments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Update the teacher's class assignment.
    [HttpPut("UpdateTeacherClassAssignment")]
    public async Task<IActionResult> UpdateTeacherClassAssignment([FromBody] UpdateTeacherAssignDto assignDto)
    {
        try
        {
            await _adminService.UpdateTeacherClassAssignmentAsync(assignDto);
            return Ok("Teacher's class assignment updated successfully.");
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest($"Validation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Remove a teacher from a class.
    [HttpDelete("DeleteTeacherFromClass")]
    public async Task<IActionResult> DeleteTeacherFromClass([FromBody] TeacherClassAssignDto assignDto)
    {
        try
        {
            await _adminService.DeleteTeacherFromClassAsync(assignDto);
            return Ok("Teacher successfully removed from the class.");
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest($"Validation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // END ASSIGN TEACHER TO CLASS

    // START CRUD SEMESTER

    // Add a new semester.
    [HttpPost("AddSemester")]
    public async Task<IActionResult> AddSemester([FromBody] SemesterDto semesterDto)
    {
        try
        {
            var semester = await _adminService.AddSemesterAsync(semesterDto);
            return Ok(semester);
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest($"Validation Error: {ex.Message}");
        }
    }

    // Update semester details.
    [HttpPut("UpdateSemester/{semesterId}")]
    public async Task<IActionResult> UpdateSemester(int semesterId, [FromBody] SemesterDto semesterDto)
    {
        try
        {
            var updatedSemester = await _adminService.UpdateSemesterAsync(semesterId, semesterDto);
            return Ok(updatedSemester);
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest($"Validation Error: {ex.Message}");
        }
    }

    // Delete a semester by its ID.
    [HttpDelete("DeleteSemesters/{semesterId}")]
    public async Task<IActionResult> DeleteSemester(int semesterId)
    {
        var success = await _adminService.DeleteSemesterAsync(semesterId);
        if (!success) return NotFound("Semester not found.");
        return Ok("Semester deleted successfully.");
    }

    // Get all semesters.
    [HttpGet("GetAllSemesters")]
    public async Task<IActionResult> GetAllSemesters()
    {
        var semesters = await _adminService.GetAllSemestersAsync();
        return Ok(semesters);
    }

    // Get a semester by ID.
    [HttpGet("GetSemesterById/{semesterId}")]
    public async Task<IActionResult> GetSemesterById(int semesterId)
    {
        try
        {
            var semester = await _adminService.GetSemesterByIdAsync(semesterId);
            return Ok(semester);
        }
        catch (AdminService.ValidateException ex)
        {
            return NotFound($"Error: {ex.Message}");
        }
    }

    // END CRUD SEMESTER

    // START CRUD CLASS

    // Add a new class.
    [HttpPost("AddClass")]
    public async Task<ActionResult<Class>> AddClass([FromBody] ClassDto newClassDto)
    {
        try
        {
            var result = await _adminService.AddClassAsync(newClassDto);
            return CreatedAtAction(nameof(GetClassById), new { id = result.ClassId }, result); // Return 201 Created
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    // Get all classes.
    [HttpGet("GetAllClasses")]
    public async Task<ActionResult<IEnumerable<Class>>> GetAllClasses()
    {
        var classes = await _adminService.GetAllClassesAsync();
        return Ok(classes);
    }

    // Get a class by its ID.
    [HttpGet("GetClass/{id}")]
    public async Task<ActionResult<Class>> GetClassById(int id)
    {
        try
        {
            var result = await _adminService.GetClassByIdAsync(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound($"Class not found: {ex.Message}");
        }
    }

    // Update a class details.
    [HttpPut("UpdateClass/{id}")]
    public async Task<IActionResult> UpdateClass(int id, [FromBody] ClassDto updatedClassDto)
    {
        try
        {
            var result = await _adminService.UpdateClassAsync(id, updatedClassDto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound($"Class not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    // Delete a class by its ID.
    [HttpDelete("DeleteClass/{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        try
        {
            var result = await _adminService.DeleteClassAsync(id);
            if (result)
                return Ok("Class deleted successfully");
            return NotFound($"Class with ID {id} not found");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    // Upgrade a student to a new class.
    [HttpPost("UpgradeClass")]
    public async Task<IActionResult> UpgradeClass([FromQuery] int oldClassId, [FromQuery] string oldAcademicYear,
        [FromQuery] int newClassId, [FromQuery] string newAcademicYear)
    {
        try
        {
            bool result =
                await _adminService.UpgradeClassAsync(oldClassId, oldAcademicYear, newClassId, newAcademicYear);
            if (result)
            {
                return Ok("Student successfully upgraded to the new class.");
            }
            else
            {
                return BadRequest("An error occurred while upgrading the class. Please try again.");
            }
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest($"Validation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // END CRUD CLASS

    // Calculate and return the class average scores.
    [HttpPost("calculate-class-average")]
    public async Task<IActionResult> CalculateClassAverageScore([FromQuery] string className,
        [FromQuery] string academicYear)
    {
        if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(academicYear))
        {
            return BadRequest("Invalid class name or academic year.");
        }

        try
        {
            var averageScoresList =
                await _adminService.CalculateAndSaveAverageScoresForClassAsync(className, academicYear);
            return Ok(averageScoresList);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    // Get all student average scores for a class and academic year.
    [HttpGet("getAverage-scores")]
    public async Task<ActionResult<IEnumerable<StudentScoreDto>>> GetStudentAverageScores(int classId,
        string academicYear)
    {
        var studentScores = await _adminService.GetStudentAverageScoresAsync(classId, academicYear);

        if (studentScores == null || !studentScores.Any())
        {
            return NotFound("No data found for the given class and academic year.");
        }

        return Ok(studentScores);
    }

    // Update class and reset scores.
    [HttpPost("UpdateClassAndResetScores")]
    public async Task<IActionResult> UpdateClassAndResetScores([FromBody] UpdateClassRequestDto request)
    {
        if (string.IsNullOrEmpty(request.CurrentAcademicYear) || string.IsNullOrEmpty(request.CurrentClassName) ||
            string.IsNullOrEmpty(request.NewAcademicYear) || string.IsNullOrEmpty(request.NewClassName))
        {
            return BadRequest(
                "Current academic year, class name, new academic year, and new class name cannot be empty.");
        }

        try
        {
            await _adminService.UpdateClassAndResetScoresAsync(request.CurrentAcademicYear, request.CurrentClassName,
                request.NewAcademicYear, request.NewClassName);
            return Ok("Class updated and scores reset successfully.");
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest($"Validation Error: {ex.Message}");
        }
        catch
        {
            return StatusCode(500, "An error occurred during processing.");
        }
    }
}