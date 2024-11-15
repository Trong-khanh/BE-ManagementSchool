using System.Net;
using System.Text.RegularExpressions;
using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace ManagementSchool.Service;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student> AddStudentWithParentAsync(StudentDtos studentDto)
    {
        // Validate inputs
        IsValidName(studentDto.FullName);
        IsValidName(studentDto.ParentName); // Kiểm tra tên của Parent

        // Kiểm tra nếu AcademicYear tồn tại trong Semester
        var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.AcademicYear == studentDto.AcademicYear);
        if (semester == null) throw new ValidateException("Invalid AcademicYear provided.");

        // Tạo và thêm student
        var student = new Student
        {
            FullName = studentDto.FullName,
            Address = studentDto.Address,
            ClassId = await _context.Classes
                .Where(c => c.ClassName == studentDto.ClassName)
                .Select(c => c.ClassId)
                .FirstOrDefaultAsync(),
            AcademicYear = studentDto.AcademicYear,
            ParentName = studentDto.ParentName
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return student;
    }

    public async Task<Student> UpdateStudentAsync(int studentId, StudentDtos studentDto)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null) throw new ValidateException("Student not found.");

        // Cập nhật thông tin student
        student.FullName = studentDto.FullName;
        student.Address = studentDto.Address;
        student.ClassId = await _context.Classes
            .Where(c => c.ClassName == studentDto.ClassName)
            .Select(c => c.ClassId)
            .FirstOrDefaultAsync();

        // Kiểm tra nếu AcademicYear tồn tại trong Semester
        var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.AcademicYear == studentDto.AcademicYear);
        if (semester == null) throw new ValidateException("Invalid AcademicYear provided.");

        student.AcademicYear = studentDto.AcademicYear;
        student.ParentName = studentDto.ParentName;

        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> DeleteStudentAsync(int studentId)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Student>> GetStudentsByClassAsync(string className, string academicYear)
    {
        // Decode the className in case it was URL-encoded.
        className = WebUtility.UrlDecode(className);

        if (string.IsNullOrWhiteSpace(className))
            throw new ArgumentException("Class name cannot be empty.");

        if (string.IsNullOrWhiteSpace(academicYear))
            throw new ArgumentException("Academic year cannot be empty.");

        // Find the class in the database.
        var classInDb = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassName == className);

        // If the class is not found, throw an exception.
        if (classInDb == null)
            throw new ArgumentException($"Class '{className}' not found.");

        // Find the students in the specified class and academic year.
        var students = await _context.Students
            .Where(s => s.ClassId == classInDb.ClassId && s.AcademicYear == academicYear)
            .ToListAsync();

        // If no students are found for the specified class and academic year,
        // you may choose to handle this case differently (e.g., return an empty list, throw an exception, etc.).
        if (students.Count == 0)
            // Here, I'm throwing an exception to indicate that no students were found for the specified criteria.
            throw new Exception($"No students found for class '{className}' in academic year '{academicYear}'.");

        return students;
    }

    public async Task<Student> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students
            .Include(s => s.Class)
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

    public async Task<IEnumerable<TeacherWithSubjectDto>> GetAllTeachersAsync()
    {
        var teachers = await _context.Teachers
            .Include(t => t.Subject)
            .ToListAsync();

        return teachers.Select(t => new TeacherWithSubjectDto
        {
            TeacherId = t.TeacherId,
            Name = t.Name,
            Email = t.Email,
            SubjectId = t.SubjectId,
            SubjectName = t.Subject.SubjectName
        });
    }

    public async Task AssignTeacherToClassAsync(TeacherClassAssignDto assignmentDto)
    {
        // Find the teacher by full name and email
        var teacher = await _context.Teachers
            .Include(t => t.Subject) // Bao gồm cả môn học
            .Include(t => t.TeacherClasses)
            .ThenInclude(tc => tc.Class)
            .FirstOrDefaultAsync(t => t.Name == assignmentDto.TeacherFullName && t.Email == assignmentDto.TeacherEmail);

        if (teacher == null)
            throw new ValidateException("Teacher with the provided full name and email not found.");

        var classEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassName == assignmentDto.ClassName);
        if (classEntity == null)
            throw new ValidateException("Class not found.");

        // Check if the teacher is already assigned to the class for that subject
        if (teacher.TeacherClasses.Any(tc => tc.Class.ClassName == assignmentDto.ClassName))
            throw new ValidateException("Teacher is already assigned to this class for their subject.");

        // Assign the teacher to the class
        var teacherClass = new TeacherClass
        {
            TeacherId = teacher.TeacherId,
            ClassId = classEntity.ClassId
        };
        _context.TeacherClasses.Add(teacherClass);
        await _context.SaveChangesAsync();
        assignmentDto.SubjectName = teacher.Subject.SubjectName;
    }

    public async Task<List<TeacherClassAssignDto>> GetTeacherClassAssignedAsync()
    {
        return await _context.TeacherClasses
            .Include(tc => tc.Teacher)
            .ThenInclude(t => t.Subject)
            .Include(tc => tc.Class)
            .Select(tc => new TeacherClassAssignDto
            {
                TeacherFullName = tc.Teacher.Name,
                TeacherEmail = tc.Teacher.Email,
                ClassName = tc.Class.ClassName,
                SubjectName = tc.Teacher.Subject.SubjectName
            })
            .ToListAsync();
    }

    public async Task UpdateTeacherClassAssignmentAsync(UpdateTeacherAssignDto assignDto)
    {
        // Tìm giáo viên bằng tên và email
        var teacher = await _context.Teachers
            .Include(t => t.TeacherClasses)
            .ThenInclude(tc => tc.Class)
            .FirstOrDefaultAsync(t => t.Name == assignDto.TeacherFullName && t.Email == assignDto.TeacherEmail);

        if (teacher == null)
            throw new ValidateException("Teacher not found.");

        // Kiểm tra lớp mà giáo viên hiện đang được gán
        var currentClassAssignment = teacher.TeacherClasses
            .FirstOrDefault(tc => tc.Class.ClassName == assignDto.CurrentClassName);

        if (currentClassAssignment == null)
            throw new ValidateException("Teacher is not assigned to this class.");

        // Tìm lớp mới để gán cho giáo viên
        var newClassEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassName == assignDto.NewClassName);

        if (newClassEntity == null)
            throw new ValidateException("New class not found.");

        // Cập nhật lớp cho giáo viên
        currentClassAssignment.ClassId = newClassEntity.ClassId;

        // Lưu thay đổi vào cơ sở dữ liệu
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTeacherFromClassAsync(TeacherClassAssignDto assignmentDto)
    {
        var teacherClass = await _context.TeacherClasses
            .Include(tc => tc.Teacher)
            .Include(tc => tc.Class)
            .FirstOrDefaultAsync(tc => tc.Teacher.Name == assignmentDto.TeacherFullName &&
                                       tc.Teacher.Email == assignmentDto.TeacherEmail &&
                                       tc.Class.ClassName == assignmentDto.ClassName);

        if (teacherClass == null)
            throw new ValidateException("Teacher is not assigned to this class.");

        _context.TeacherClasses.Remove(teacherClass);
        await _context.SaveChangesAsync();
    }

    public async Task<Semester> AddSemesterAsync(SemesterDto semesterDto)
    {
        if (semesterDto == null)
            throw new ValidateException("The semesterDto field is required.");

        // Validate StartDate and EndDate
        if (semesterDto.EndDate <= semesterDto.StartDate)
            throw new ValidateException("EndDate must be greater than StartDate.");

        // Convert SemesterType string to enum
        if (!Enum.TryParse<SemesterType>(semesterDto.SemesterType.Replace(" ", ""), out var semesterType))
            throw new ValidateException("Invalid SemesterType specified.");

        var semester = new Semester
        {
            SemesterType = semesterType,
            StartDate = semesterDto.StartDate,
            EndDate = semesterDto.EndDate,
            AcademicYear = semesterDto.AcademicYear
        };

        _context.Semesters.Add(semester);
        await _context.SaveChangesAsync();

        return semester;
    }

    public async Task<Semester> UpdateSemesterAsync(int semesterId, SemesterDto semesterDto)
    {
        if (semesterDto == null)
            throw new ValidateException("The semesterDto object must be provided.");

        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null)
            throw new ValidateException("Semester not found.");

        // Validate StartDate and EndDate
        if (semesterDto.StartDate >= semesterDto.EndDate)
            throw new ValidateException("StartDate must be before EndDate.");

        // Convert SemesterType string to enum
        if (!Enum.TryParse<SemesterType>(semesterDto.SemesterType.Replace(" ", ""), out var semesterType))
            throw new ValidateException("Invalid SemesterType specified.");

        semester.SemesterType = semesterType; // Update enum
        semester.StartDate = semesterDto.StartDate;
        semester.EndDate = semesterDto.EndDate;
        semester.AcademicYear = semesterDto.AcademicYear;

        await _context.SaveChangesAsync();

        return semester;
    }

    public async Task<bool> DeleteSemesterAsync(int semesterId)
    {
        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null) return false;

        _context.Semesters.Remove(semester);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<SemesterDto>> GetAllSemestersAsync()
    {
        return await _context.Semesters
            .Select(s => new SemesterDto
            {
                SemesterId = s.SemesterId, // Use SemesterId instead of Id
                SemesterType = s.SemesterType.GetDisplayName(),
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                AcademicYear = s.AcademicYear
            })
            .ToListAsync();
    }

    public async Task<SemesterDto> GetSemesterByIdAsync(int semesterId)
    {
        // Find the semester using the provided semesterId
        var semester = await _context.Semesters.FindAsync(semesterId);

        // Check if the semester was found
        if (semester == null)
            throw new ValidateException("Semester not found.");

        // Return a new SemesterDto with the details
        return new SemesterDto
        {
            SemesterType = semester.SemesterType.ToString(),
            StartDate = semester.StartDate,
            EndDate = semester.EndDate,
            AcademicYear = semester.AcademicYear
        };
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _context.Students
            .Include(s => s.Class) // Include the related class
            .ToListAsync();
    }

    public async Task<Class> AddClassAsync(ClassDto newClassDto)
    {
        if (string.IsNullOrWhiteSpace(newClassDto.ClassName))
            throw new ArgumentException("Class name cannot be empty.");

        var newClass = new Class
        {
            ClassName = newClassDto.ClassName
        };

        _context.Classes.Add(newClass);
        await _context.SaveChangesAsync();
        return newClass;
    }

    public async Task<Class> UpdateClassAsync(int classId, ClassDto updatedClassDto)
    {
        var existingClass = await _context.Classes.FindAsync(classId);
        if (existingClass == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found.");

        existingClass.ClassName = updatedClassDto.ClassName;
        await _context.SaveChangesAsync();
        return existingClass;
    }

    public async Task<bool> DeleteClassAsync(int classId)
    {
        var classToDelete = await _context.Classes.FindAsync(classId);
        if (classToDelete == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found.");

        _context.Classes.Remove(classToDelete);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Class>> GetAllClassesAsync()
    {
        return await _context.Classes.ToListAsync();
    }

    public async Task<Class> GetClassByIdAsync(int classId)
    {
        var result = await _context.Classes.FindAsync(classId);
        if (result == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found.");
        return result;
    }

    public async Task<bool> UpgradeClassAsync(int oldClassId, string oldAcademicYear, int newClassId,
        string newAcademicYear)
    {
        // Check new academic year exists
        var semesterExists = await _context.Semesters.AnyAsync(s => s.AcademicYear == newAcademicYear);
        if (!semesterExists)
        {
            throw new ValidateException("New academic year does not exist.");
        }

        var summaries = await _context.SummariesOfYear
            .Include(s => s.Student)
            .Where(s => s.Student.ClassId == oldClassId && s.AcademicYear == oldAcademicYear)
            .ToListAsync();

        if (summaries.Count == 0)
        {
            throw new ValidateException("Class or Academic Year does not exist.");
        }

        foreach (var summary in summaries)
        {
            if (summary.Status == "Next Class")
            {
                summary.Student.ClassId = newClassId;
                summary.Student.AcademicYear = newAcademicYear;
            }
            else if (summary.Status == "Resit")
            {
                summary.Student.AcademicYear = newAcademicYear;
            }
        }

        await _context.SaveChangesAsync();
        return true;
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

public async Task<List<AverageScore>> CalculateAndSaveAverageScoresForClassAsync(string className, string academicYear)
{
    // Log to check the status
    Console.WriteLine($"Bắt đầu tính điểm cho lớp {className} - Năm học {academicYear}");

    // Fetch the class entity by class name
    var classEntity = await _context.Classes
        .FirstOrDefaultAsync(c => c.ClassName == className);
    if (classEntity == null)
    {
        throw new Exception("Class not found");
    }

    // Fetch all students in the class for the given academic year
    var studentsInClass = await _context.Students
        .Where(s => s.ClassId == classEntity.ClassId && s.AcademicYear == academicYear)
        .ToListAsync();

    var averageScoresList = new List<AverageScore>();

    foreach (var student in studentsInClass)
    {
        Console.WriteLine($"Đang tính điểm cho học sinh: {student.StudentId}");

        // Fetch the average scores for the student for the given academic year
        var scores = await _context.SubjectsAverageScores
            .Where(s => s.StudentId == student.StudentId && s.AcademicYear == academicYear)
            .ToListAsync();

        var semester1Scores = scores.Where(s => s.SemesterAverage1.HasValue).ToList();
        var semester2Scores = scores.Where(s => s.SemesterAverage2.HasValue).ToList();

        // Calculate the averages for Semester 1, Semester 2, and Academic Year
        decimal? averageSemester1 = semester1Scores.Count == 12
            ? Math.Round((decimal)(semester1Scores.Sum(s => s.SemesterAverage1.Value) / 12), 1)
            : (decimal?)null;

        decimal? averageSemester2 = semester2Scores.Count == 12
            ? Math.Round((decimal)(semester2Scores.Sum(s => s.SemesterAverage2.Value) / 12), 1)
            : (decimal?)null;

        decimal? averageAcademicYear = (averageSemester1.HasValue && averageSemester2.HasValue)
            ? Math.Round((decimal)((averageSemester1 + (averageSemester2 * 2)) / 3), 1)
            : (decimal?)null;

        // Convert the decimal? values back to double? before assigning to AverageScore properties
        double? finalAverageSemester1 = averageSemester1.HasValue ? (double?)averageSemester1.Value : null;
        double? finalAverageSemester2 = averageSemester2.HasValue ? (double?)averageSemester2.Value : null;
        double? finalAverageAcademicYear = averageAcademicYear.HasValue ? (double?)averageAcademicYear.Value : null;

        // Find the existing average score or create a new one if it doesn't exist
        var averageScore = await _context.AverageScores
            .FirstOrDefaultAsync(a => a.StudentId == student.StudentId && a.Student.AcademicYear == academicYear);

        if (averageScore == null)
        {
            Console.WriteLine($"Thêm bản ghi mới cho học sinh {student.StudentId}");
            averageScore = new AverageScore
            {
                StudentId = student.StudentId,
                FullName = student.FullName,  // Ensure FullName is populated
                AverageSemester1 = finalAverageSemester1,
                AverageSemester2 = finalAverageSemester2,
                AverageAcademicYear = finalAverageAcademicYear
            };
            _context.AverageScores.Add(averageScore);
        }
        else
        {
            Console.WriteLine($"Cập nhật bản ghi cho học sinh {student.StudentId}");
            averageScore.AverageSemester1 = finalAverageSemester1;
            averageScore.AverageSemester2 = finalAverageSemester2;
            averageScore.AverageAcademicYear = finalAverageAcademicYear;
            averageScore.FullName = student.FullName;  
        }

        // Add the calculated average score to the result list
        averageScoresList.Add(averageScore);
    }

    // Save all changes to the database
    await _context.SaveChangesAsync();
    Console.WriteLine("Đã lưu tất cả thay đổi vào cơ sở dữ liệu.");

    // Return the list of calculated and saved average scores
    return averageScoresList;
}



public async Task<IEnumerable<StudentScoreDto>> GetStudentAverageScoresAsync(int classId, string academicYear)
{
    var studentScores = await _context.AverageScores
        .Include(a => a.Student) 
        .Include(a => a.Student.Class) 
        .Where(a => a.Student.ClassId == classId && a.Student.AcademicYear == academicYear) 
        .Select(a => new StudentScoreDto
        {
            StudentFullName = a.Student.FullName, 
            SemesterAverage1 = a.AverageSemester1, 
            SemesterAverage2 = a.AverageSemester2, 
            AnnualAverage = a.AverageAcademicYear,
            AcademicYear = a.Student.AcademicYear 
        })
        .ToListAsync();

    return studentScores;
}
    public class ValidateException : Exception
    {
        public ValidateException(string message) : base(message)
        {
        }
    }
}
