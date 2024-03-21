using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementSchool.Service
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
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

                    // Now create and add the student with the ParentId from the existing/new parent
                    var student = new Student
                    {
                        FullName = studentDto.FullName,
                        Address = studentDto.Address,
                        ClassId = await _context.Classes
                            .Where(c => c.ClassName == studentDto.ClassName)
                            .Select(c => c.ClassId)
                            .FirstOrDefaultAsync(),
                        ParentId = parent.ParentId // Use the existing/new parent's ID
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

    }
}
