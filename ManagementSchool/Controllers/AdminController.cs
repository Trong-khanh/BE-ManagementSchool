using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagementSchool.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
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

        [HttpDelete("DeleteStudent/{studentId}")]
        public async Task<IActionResult> DeleteStudent(int studentId)
        {
            var result = await _studentService.DeleteStudentAsync(studentId);
            if (!result) return NotFound("Student not found.");
            return Ok();
        }

        [HttpPut("UpdateStudent/{studentId}")]
        public async Task<IActionResult> UpdateStudent(int studentId, [FromBody] StudentDtos studentDtos)
        {
            try
            {
                var updatedStudent = await _studentService.UpdateStudentAsync(studentId, studentDtos);
                return Ok(updatedStudent);
            }
            catch (StudentService.ValidateException ex)
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

                var students = await _studentService.GetStudentsByClassAsync(className);
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

                var students = await _studentService.GetStudentsBySchoolYearAsync(yearName);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}