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
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("getaAllStudents")]
    public async Task<IActionResult> GetAllStudents()
    {
        return Ok(await _adminService.GetAllStudentsAsync());
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

    // END CRUD TEACHER

    //====================================================================================================
    //====================================================================================================

    // ASSIGN TEACHER TO CLASS

    [HttpPost("AssignTeacherToClass")]
    public async Task<IActionResult> AssignTeacherToClass([FromBody] TeacherClassAssignDto assigntDto)
    {
        try
        {
            await _adminService.AssignTeacherToClassAsync(assigntDto);
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
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

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
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpDelete("DeleteTeacherFromClass")]
    public async Task<IActionResult> DeleteTeacherFromClass([FromBody] TeacherClassAssignDto assignDto)
    {
        try
        {
            await _adminService.DeleteTeacherFromClassAsync(assignDto);
            return Ok("Teacher removed from class successfully.");
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


    // END ASSIGN TEACHER TO CLASS

    // START CRUD SEMESTER//

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
            return BadRequest(ex.Message);
        }
    }

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
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("DeleteSemesters/{semesterId}")]
    public async Task<IActionResult> DeleteSemester(int semesterId)
    {
        var success = await _adminService.DeleteSemesterAsync(semesterId);
        if (!success) return NotFound("Semester not found.");
        return Ok();
    }

    [HttpGet("GetAllSemesters")]
    public async Task<IActionResult> GetAllSemesters()
    {
        var semesters = await _adminService.GetAllSemestersAsync();
        return Ok(semesters);
    }

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
            return NotFound(ex.Message);
        }
    }

    // Create
    [HttpPost("AddClass")]
    public async Task<ActionResult<Class>> AddClass([FromBody] ClassDto newClassDto)
    {
        try
        {
            var result = await _adminService.AddClassAsync(newClassDto);
            return CreatedAtAction(nameof(GetClassById), new { id = result.ClassId }, result); // Trả về 201 Created
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Read All
    [HttpGet("GetAllClasses")]
    public async Task<ActionResult<IEnumerable<Class>>> GetAllClasses()
    {
        var classes = await _adminService.GetAllClassesAsync();
        return Ok(classes);
    }

    // Read One
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
            return NotFound(ex.Message);
        }
    }

    // Update
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
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Delete
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
            return BadRequest(ex.Message);
        }
    }


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
                return Ok("The student was next new class successfully .");
            }
            else
            {
                return BadRequest("The error occurred while upgrading the class. Please try agai.");
            }
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred in processing the request: {ex.Message}");
        }
    }


// Endpoint Controller để tính điểm trung bình và trả về kết quả
    [HttpPost("calculate-class-average")]
    public async Task<IActionResult> CalculateClassAverageScore([FromQuery] string className, [FromQuery] string academicYear)
    {
        if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(academicYear))
        {
            return BadRequest("Class Name or AcademicYear Invalid.");
        }

        try
        {
            // Tính toán và nhận danh sách kết quả từ CalculateAndSaveAverageScoresForClassAsync
            var averageScoresList = await _adminService.CalculateAndSaveAverageScoresForClassAsync(className, academicYear);

            // Trả về kết quả dưới dạng JSON
            return Ok(averageScoresList);
        }
        catch (Exception ex)
        {
            return BadRequest($"Lỗi: {ex.Message}");
        }
    }
    
    [HttpGet("getAverage-scores")]
    public async Task<ActionResult<IEnumerable<StudentScoreDto>>> GetStudentAverageScores(int classId, string academicYear)
    {
        var studentScores = await _adminService.GetStudentAverageScoresAsync(classId, academicYear);

        if (studentScores == null || !studentScores.Any())
        {
            return NotFound("No data found for the given class and academic year.");
        }

        return Ok(studentScores);
    }
    
// Endpoint để cập nhật lớp học và reset điểm
    [HttpPost("UpdateClassAndResetScores")]
    public async Task<IActionResult> UpdateClassAndResetScores([FromBody] UpdateClassRequestDto request)
    {
        if (string.IsNullOrEmpty(request.CurrentAcademicYear) || string.IsNullOrEmpty(request.CurrentClassName) || 
            string.IsNullOrEmpty(request.NewAcademicYear) || string.IsNullOrEmpty(request.NewClassName))
        {
            return BadRequest("Thông tin năm học hiện tại, lớp hiện tại, năm học mới và lớp mới không được để trống.");
        }

        try
        {
            await _adminService.UpdateClassAndResetScoresAsync(request.CurrentAcademicYear, request.CurrentClassName, request.NewAcademicYear, request.NewClassName);
            return Ok("Cập nhật lớp học và reset điểm thành công.");
        }
        catch (AdminService.ValidateException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return StatusCode(500, "Đã xảy ra lỗi trong quá trình xử lý.");
        }
    }

    
}