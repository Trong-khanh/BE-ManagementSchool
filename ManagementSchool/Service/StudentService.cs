using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementSchool.Service;

public class StudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student> AddStudentAsync(StudentDtos studentDto)
    {
        var classEntity = await _context.Classes.FirstOrDefaultAsync(c => c.ClassName == studentDto.ClassName);
        var parentEntity = await _context.Parents.FirstOrDefaultAsync(p => p.ParentName == studentDto.ParentName);

        if (classEntity == null || parentEntity == null)
        {
            throw new Exception("Class or parent not found");
        }

        var student = new Student
        {
            FullName = studentDto.FullName,
            Email = studentDto.Email,
            Address = studentDto.Address,
            ClassId = classEntity.ClassId,
            ParentId = parentEntity.ParentId
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return student;
    }
}