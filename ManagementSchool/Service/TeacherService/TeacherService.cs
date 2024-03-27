using ManagementSchool.Dto;
using ManagementSchool.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementSchool.Models;

namespace ManagementSchool.Service.TeacherService
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _dbContext;

        public TeacherService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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

            // Kiểm tra xem giáo viên đã được gán để dạy môn học nào chưa
            var existingTeacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.SubjectId == teacherDto.SubjectId);
            if (existingTeacher != null)
            {
                // Nếu đã có giáo viên được gán cho môn học này, thông báo lỗi
                throw new InvalidOperationException("A teacher is already assigned to teach this subject.");
            }

            // Kiểm tra môn học tồn tại
            var subject = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.SubjectId == teacherDto.SubjectId);
            if (subject == null)
                throw new ArgumentException($"Subject with ID '{teacherDto.SubjectId}' does not exist.");

            var teacher = new Teacher
            {
                Name = teacherDto.Name,
                Email = teacherDto.Email,
                SubjectId = teacherDto.SubjectId.Value
            };

            _dbContext.Teachers.Add(teacher);
            await _dbContext.SaveChangesAsync();

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
            var teacher = await _dbContext.Teachers.FindAsync(teacherId);
            if (teacher == null)
                return false;

            _dbContext.Teachers.Remove(teacher);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTeacherByNameAsync(string teacherName)
        {
            var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Name == teacherName);
            if (teacher == null)
                return false;

            _dbContext.Teachers.Remove(teacher);
            await _dbContext.SaveChangesAsync();

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

            var teacher = await _dbContext.Teachers
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
            if (teacher == null)
                throw new ArgumentException($"Teacher with ID {teacherId} does not exist.");

            var subject = await _dbContext.Subjects.FindAsync(teacherDto.SubjectId);
            if (subject == null)
                throw new ArgumentException($"Subject with ID {teacherDto.SubjectId} does not exist.");

            teacher.Name = teacherDto.Name;
            teacher.Email = teacherDto.Email;
            teacher.SubjectId = teacherDto.SubjectId.Value;
            teacher.Subject = subject;

            await _dbContext.SaveChangesAsync();

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
            var teachers = await _dbContext.Teachers
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

            var teachers = await _dbContext.Teachers
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
}
