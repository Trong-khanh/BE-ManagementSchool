using ManagementSchool.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ManagementSchool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("GetStudentInfo/{studentId}")]
        public async Task<IActionResult> GetStudentInfo(int studentId) 
        {
            try
            {
                var student = await _studentService.GetStudentByIdAsync(studentId);
                if (student == null)
                    return NotFound("Student not found.");

                return Ok(new
                {
                    FullName = student.FullName,
                    ClassName = student.Class.ClassName,
                    Address = student.Address,
                    ParentName = student.Parent.ParentName
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
