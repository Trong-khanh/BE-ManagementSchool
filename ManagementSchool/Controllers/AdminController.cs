using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ManagementSchool.Controllers;

[Authorize(Roles = "Admin")]
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

    [HttpPost("AddStudent")]
    public async Task<IActionResult> AddStudent([FromBody] StudentDtos studentDtos)
    {
        var student = await _adminService.AddStudentWithParentAsync(studentDtos);
        return Ok(student);
    }

    [HttpDelete("DeleteStudent/{studentId}")]
    public async Task<IActionResult> DeleteStudent(int studentId)
    {
        var result = await _adminService.DeleteStudentAsync(studentId);
        if (!result) return NotFound("Student not found.");
        return Ok();
    }

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
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetStudentsByClass/{className}")]
    public async Task<IActionResult> GetStudentsByClass(string className)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(className))
                return BadRequest("Class name cannot be empty.");

            var students = await _adminService.GetStudentsByClassAsync(className);
            return Ok(students);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("GetStudentsBySchoolYear/{yearName}")]
    public async Task<IActionResult> GetStudentsBySchoolYear(string yearName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(yearName))
                return BadRequest("Year name cannot be empty.");

            var students = await _adminService.GetStudentsBySchoolYearAsync(yearName);
            return Ok(students);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    // END CRUD STUDENT 

    //====================================================================================================
    //====================================================================================================

    // START CRUD TEACHER

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
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpDelete("DeleteTeacher/{teacherId}")]
    public async Task<IActionResult> DeleteTeacher(int teacherId)
    {
        try
        {
            var result = await _adminService.DeleteTeacherAsync(teacherId);
            if (!result) return NotFound("Teacher not found.");
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

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
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

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
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpDelete("DeleteTeacherByName/{teacherName}")]
    public async Task<IActionResult> DeleteTeacherByName(string teacherName)
    {
        try
        {
            var result = await _adminService.DeleteTeacherByNameAsync(teacherName);
            if (!result) return NotFound("Teacher not found.");
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpGet("GetTeachersBySubject/{subjectName}")]
    public async Task<IActionResult> GetTeachersBySubject(string subjectName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(subjectName))
                return BadRequest("Subject name cannot be empty.");

            var teachers = await _adminService.GetTeachersBySubjectAsync(subjectName);
            return Ok(teachers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    // END CRUD TEACHER
    
    //====================================================================================================
    //====================================================================================================
    
    // ASSIGN TEACHER TO CLASS
    
    [HttpPost("AssignTeacherToClass")]
    public async Task<IActionResult> AssignTeacherToClass([FromBody] TeacherClassAssignmentDto assignmentDto)
    {
        try
        {
            await _adminService.AssignTeacherToClassAsync(assignmentDto);
            return Ok("Teacher assigned to class successfully.");
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
    
    [HttpGet("GetTeacherClassAssignments")]
    public async Task<IActionResult> GetTeacherClassAssignments()
    {
        try
        {
            var assignments = await _adminService.GetTeacherClassAssignmentsAsync();
            return Ok(assignments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

}