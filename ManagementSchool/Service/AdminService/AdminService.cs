using ManagementSchool.Dto;
using ManagementSchool.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ManagementSchool.Models;

namespace ManagementSchool.Service;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public class ValidateException : Exception
    {
        public ValidateException(string message) : base(message)
        {
        }
    }

    public bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidateException("Name cannot be empty.");

        // Regex pattern for validating name (adjust according to your requirements)
        var nameRegex = @"^[a-zA-Z\s]+$";
        if (!Regex.IsMatch(name, nameRegex))
            throw new ValidateException("Name contains invalid characters.");

        return true;
    }

    public async Task<Student> AddStudentWithParentAsync(StudentDtos studentDto)
    {
        // Validate inputs
        IsValidName(studentDto.FullName);
        IsValidName(studentDto.ParentName); // Make sure the parent's name is also valid

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Check if the parent already exists by name or some unique identifier
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.ParentName == studentDto.ParentName);

                // If the parent doesn't exist, create and add the new parent
                if (parent == null)
                {
                    parent = new Parent
                    {
                        ParentName = studentDto.ParentName
                    };
                    _context.Parents.Add(parent);
                    await _context.SaveChangesAsync(); // Save to get the new ParentId
                }

                // Create and add the student with the ParentId from the existing/new parent
                var student = new Student
                {
                    FullName = studentDto.FullName,
                    Address = studentDto.Address,
                    ClassId = await _context.Classes
                        .Where(c => c.ClassName == studentDto.ClassName)
                        .Select(c => c.ClassId)
                        .FirstOrDefaultAsync(),
                    ParentId = parent.ParentId
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                // Commit transaction if all commands succeed
                await transaction.CommitAsync();

                return student;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any exception occurs
                await transaction.RollbackAsync();
                throw new ValidateException($"An error occurred while adding the student and parent: {ex.Message}");
            }
        }
    }

    public async Task<bool> DeleteStudentAsync(int studentId)
    {
        var student = await _context.Students.Include(s => s.Parent)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null) return false;

        // Check if the parent is associated with any other students
        var otherStudents = await _context.Students
            .Where(s => s.ParentId == student.ParentId && s.StudentId != studentId)
            .ToListAsync();

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // If the parent is not associated with any other students, delete the parent
                if (!otherStudents.Any())
                {
                    var parent = await _context.Parents.FindAsync(student.ParentId);
                    if (parent != null) _context.Parents.Remove(parent);
                }

                // Delete the student
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                // Commit transaction if all commands succeed
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any exception occurs
                await transaction.RollbackAsync();
                throw; // or log the exception as needed
            }
        }
    }


    public async Task<Student> UpdateStudentAsync(int studentId, StudentDtos studentDto)
    {
        var student = await _context.Students.Include(s => s.Parent)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null) throw new ValidateException("Student not found.");

        // Update student and parent details
        student.FullName = studentDto.FullName;
        student.Address = studentDto.Address;
        student.ClassId = await _context.Classes.Where(c => c.ClassName == studentDto.ClassName)
            .Select(c => c.ClassId).FirstOrDefaultAsync();
        student.Parent.ParentName = studentDto.ParentName;

        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<IEnumerable<Student>> GetStudentsByClassAsync(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            throw new ArgumentException("Class name cannot be empty.");

        // encryption  className before send to  method service
        className = Uri.EscapeDataString(className);
        var classInDb = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassName == className);

        if (classInDb == null)
            throw new ArgumentException($"Class '{className}' not found.");

        return await _context.Students
            .Where(s => s.ClassId == classInDb.ClassId)
            .ToListAsync();
    }


    public async Task<IEnumerable<Student>> GetStudentsBySchoolYearAsync(string YearName)
    {
        if (string.IsNullOrWhiteSpace(YearName))
            throw new ArgumentException("Year name cannot be empty.");

        return await _context.Students
            .Include(s => s.Class)
            .ThenInclude(c => c.SchoolYear)
            .Where(s => s.Class.SchoolYear.YearName == YearName)
            .ToListAsync();
    }

    public async Task<Student> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students
            .Include(s => s.Class)
            .Include(s => s.Parent)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public async Task<TeacherDto?> AddTeacherAsync(TeacherDto teacherDto)
    {
        if (teacherDto == null)
            throw new ArgumentNullException(nameof(teacherDto));

        if (string.IsNullOrWhiteSpace(teacherDto.Name))
            throw new ArgumentException("Teacher name cannot be empty or whitespace.", nameof(teacherDto.Name));

        if (string.IsNullOrWhiteSpace(teacherDto.Email))
            throw new ArgumentException("Teacher email cannot be empty or whitespace.", nameof(teacherDto.Email));

        if (teacherDto.SubjectId == null)
            throw new ArgumentException("SubjectId cannot be null.");

        // Check if teachers have been assigned to teach any subjects that are not yet available
        var existingTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.SubjectId == teacherDto.SubjectId);
        if (existingTeacher != null)
            // If there is already a teacher assigned to this subject, report an error
            throw new InvalidOperationException("A teacher is already assigned to teach this subject.");

        // Check if the subject exists
        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == teacherDto.SubjectId);
        if (subject == null)
            throw new ArgumentException($"Subject with ID '{teacherDto.SubjectId}' does not exist.");

        var teacher = new Teacher
        {
            Name = teacherDto.Name,
            Email = teacherDto.Email,
            SubjectId = teacherDto.SubjectId.Value
        };

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        return new TeacherDto
        {
            Name = teacher.Name,
            Email = teacher.Email,
            SubjectId = teacher.SubjectId,
            SubjectName = subject.SubjectName
        };
    }

    public async Task<bool> DeleteTeacherAsync(int teacherId)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
            return false;

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteTeacherByNameAsync(string teacherName)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Name == teacherName);
        if (teacher == null)
            return false;

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TeacherDto?> UpdateTeacherAsync(int teacherId, TeacherDto teacherDto)
    {
        if (teacherDto == null)
            throw new ArgumentNullException(nameof(teacherDto));

        if (string.IsNullOrWhiteSpace(teacherDto.Name))
            throw new ArgumentException("Teacher name cannot be empty or whitespace.", nameof(teacherDto.Name));

        if (string.IsNullOrWhiteSpace(teacherDto.Email))
            throw new ArgumentException("Teacher email cannot be empty or whitespace.", nameof(teacherDto.Email));

        var teacher = await _context.Teachers
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
        if (teacher == null)
            throw new ArgumentException($"Teacher with ID {teacherId} does not exist.");

        var subject = await _context.Subjects.FindAsync(teacherDto.SubjectId);
        if (subject == null)
            throw new ArgumentException($"Subject with ID {teacherDto.SubjectId} does not exist.");

        teacher.Name = teacherDto.Name;
        teacher.Email = teacherDto.Email;
        teacher.SubjectId = teacherDto.SubjectId.Value;
        teacher.Subject = subject;

        await _context.SaveChangesAsync();

        var result = new TeacherDto
        {
            Name = teacher.Name,
            Email = teacher.Email,
            SubjectId = teacher.SubjectId,
            SubjectName = teacher.Subject.SubjectName
        };
        return result;
    }

    public async Task<IEnumerable<TeacherDto>> GetAllTeachersAsync()
    {
        var teachers = await _context.Teachers
            .Include(t => t.Subject)
            .ToListAsync();

        return teachers.Select(t => new TeacherDto
        {
            Name = t.Name,
            Email = t.Email,
            SubjectId = t.SubjectId,
            SubjectName = t.Subject.SubjectName
        });
    }

    public async Task<IEnumerable<TeacherDto>> GetTeachersBySubjectAsync(string subjectName)
    {
        if (string.IsNullOrWhiteSpace(subjectName))
            throw new ArgumentException("Subject name cannot be empty or whitespace.", nameof(subjectName));

        var teachers = await _context.Teachers
            .Include(t => t.Subject)
            .Where(t => t.Subject.SubjectName.ToLower() == subjectName.ToLower())
            .ToListAsync();

        return teachers.Select(t => new TeacherDto
        {
            Name = t.Name,
            Email = t.Email,
            SubjectId = t.SubjectId,
            SubjectName = t.Subject.SubjectName
        });
    }
}