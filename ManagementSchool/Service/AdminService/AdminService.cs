using System.Net;
using System.Text.RegularExpressions;
using ManagementSchool.Dto;
using ManagementSchool.Entities;
using ManagementSchool.Models;
using ManagementSchool.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Adds a new student with parent details
    public async Task<Student> AddStudentWithParentAsync(StudentDtos studentDto)
    {
        // Validate inputs
        if (!IsValidName(studentDto.FullName) || !IsValidName(studentDto.ParentName))
            throw new ValidateException("Name contains invalid characters.");
        if (!IsValidEmail(studentDto.ParentEmail))
            throw new ValidateException("Invalid email format.");

        // Check if AcademicYear exists
        var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.AcademicYear == studentDto.AcademicYear);
        if (semester == null)
            throw new ValidateException("The provided AcademicYear does not exist.");

        // Get ClassId
        var classId = await _context.Classes
            .Where(c => c.ClassName == studentDto.ClassName)
            .Select(c => c.ClassId)
            .FirstOrDefaultAsync();
        if (classId == 0)
            throw new ValidateException("Class not found.");

        // Create and add the new student
        var student = new Student
        {
            FullName = studentDto.FullName,
            Address = studentDto.Address,
            ClassId = classId,
            AcademicYear = studentDto.AcademicYear,
            ParentName = studentDto.ParentName,
            ParentEmail = studentDto.ParentEmail
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return student;
    }

    // Updates the student details
    public async Task<Student> UpdateStudentAsync(int studentId, StudentDtos studentDto)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null)
            throw new ValidateException("Student not found.");

        if (!IsValidName(studentDto.FullName) || !IsValidName(studentDto.ParentName))
            throw new ValidateException("Name contains invalid characters.");
        if (!IsValidEmail(studentDto.ParentEmail))
            throw new ValidateException("Invalid email format.");

        var classId = await _context.Classes
            .Where(c => c.ClassName == studentDto.ClassName)
            .Select(c => c.ClassId)
            .FirstOrDefaultAsync();
        if (classId == 0)
            throw new ValidateException("Class not found.");

        var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.AcademicYear == studentDto.AcademicYear);
        if (semester == null)
            throw new ValidateException("The provided AcademicYear does not exist.");

        student.FullName = studentDto.FullName;
        student.Address = studentDto.Address;
        student.ClassId = classId;
        student.AcademicYear = studentDto.AcademicYear;
        student.ParentName = studentDto.ParentName;
        student.ParentEmail = studentDto.ParentEmail;

        await _context.SaveChangesAsync();
        return student;
    }

    // Deletes a student by their ID
    public async Task<bool> DeleteStudentAsync(int studentId)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return true;
    }

    // Fetches students by class name and academic year
    public async Task<IEnumerable<Student>> GetStudentsByClassAsync(string className, string academicYear)
    {
        // Decode URL-encoded class name
        className = WebUtility.UrlDecode(className);

        if (string.IsNullOrWhiteSpace(className))
            throw new ArgumentException("Class name cannot be empty.");

        if (string.IsNullOrWhiteSpace(academicYear))
            throw new ArgumentException("Academic year cannot be empty.");

        // Find the class in the database
        var classInDb = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassName == className);

        if (classInDb == null)
            throw new ArgumentException($"Class '{className}' not found.");

        // Get the students for the specified class and academic year
        var students = await _context.Students
            .Where(s => s.ClassId == classInDb.ClassId && s.AcademicYear == academicYear)
            .ToListAsync();

        if (students.Count == 0)
            throw new Exception($"No students found for class '{className}' in academic year '{academicYear}'.");

        return students;
    }

    // Adds a teacher and associates with a subject
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

    // Deletes a teacher by their ID
    public async Task<bool> DeleteTeacherAsync(int teacherId)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
            return false;

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();

        return true;
    }

    // Updates teacher details
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

    // Fetches all teachers with their subject details
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

    // Assigns a teacher to a class
    public async Task AssignTeacherToClassAsync(TeacherClassAssignDto assignmentDto)
    {
        // Find teacher by full name and email
        var teacher = await _context.Teachers
            .Include(t => t.Subject)
            .Include(t => t.TeacherClasses)
            .ThenInclude(tc => tc.Class)
            .FirstOrDefaultAsync(t => t.Name == assignmentDto.TeacherFullName && t.Email == assignmentDto.TeacherEmail);

        if (teacher == null)
            throw new ValidateException("Teacher with the provided name and email not found.");

        var classEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassName == assignmentDto.ClassName);
        if (classEntity == null)
            throw new ValidateException("Class not found.");

        // Check if teacher is already assigned to this class
        if (teacher.TeacherClasses.Any(tc => tc.Class.ClassName == assignmentDto.ClassName))
            throw new ValidateException("Teacher is already assigned to this class.");

        // Assign teacher to the class
        var teacherClass = new TeacherClass
        {
            TeacherId = teacher.TeacherId,
            ClassId = classEntity.ClassId
        };
        _context.TeacherClasses.Add(teacherClass);
        await _context.SaveChangesAsync();
        assignmentDto.SubjectName = teacher.Subject.SubjectName;
    }

    // Gets all teacher assignments to classes
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
                SubjectName = tc.Teacher.Subject.SubjectName,
                ClassName = tc.Class.ClassName
            })
            .ToListAsync();
    }

    public async Task UpdateTeacherClassAssignmentAsync(UpdateTeacherAssignDto assignDto)
    {
        // Find the teacher by full name and email
        var teacher = await _context.Teachers
            .Include(t => t.TeacherClasses)
            .ThenInclude(tc => tc.Class)
            .FirstOrDefaultAsync(t => t.Name == assignDto.TeacherFullName && t.Email == assignDto.TeacherEmail);

        // If the teacher is not found, throw an error
        if (teacher == null)
            throw new ValidateException("Teacher not found.");

        // Check if the teacher is currently assigned to the class
        var currentClassAssignment = teacher.TeacherClasses
            .FirstOrDefault(tc => tc.Class.ClassName == assignDto.CurrentClassName);

        // If the teacher is not assigned to the class, throw an error
        if (currentClassAssignment == null)
            throw new ValidateException("Teacher is not assigned to this class.");

        // Find the new class to assign to the teacher
        var newClassEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassName == assignDto.NewClassName);

        // If the new class is not found, throw an error
        if (newClassEntity == null)
            throw new ValidateException("New class not found.");

        // Update the teacher's class assignment
        currentClassAssignment.ClassId = newClassEntity.ClassId;

        // Save changes to the database
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTeacherFromClassAsync(TeacherClassAssignDto assignmentDto)
    {
        // Find the teacher-class assignment
        var teacherClass = await _context.TeacherClasses
            .Include(tc => tc.Teacher)
            .Include(tc => tc.Class)
            .FirstOrDefaultAsync(tc => tc.Teacher.Name == assignmentDto.TeacherFullName &&
                                       tc.Teacher.Email == assignmentDto.TeacherEmail &&
                                       tc.Class.ClassName == assignmentDto.ClassName);

        // If the teacher is not assigned to the class, throw an error
        if (teacherClass == null)
            throw new ValidateException("Teacher is not assigned to this class.");

        // Remove the teacher from the class
        _context.TeacherClasses.Remove(teacherClass);
        await _context.SaveChangesAsync();
    }

    public async Task<Semester> AddSemesterAsync(SemesterDto semesterDto)
    {
        // Ensure semesterDto is not null
        if (semesterDto == null)
            throw new ValidateException("The semesterDto field is required.");

        // Validate that the end date is after the start date
        if (semesterDto.EndDate <= semesterDto.StartDate)
            throw new ValidateException("EndDate must be greater than StartDate.");

        // Convert SemesterType string to enum
        if (!Enum.TryParse<SemesterType>(semesterDto.SemesterType.Replace(" ", ""), out var semesterType))
            throw new ValidateException("Invalid SemesterType specified.");

        // Create a new semester and save to the database
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
        // Ensure semesterDto is not null
        if (semesterDto == null)
            throw new ValidateException("The semesterDto object must be provided.");

        // Find the semester by ID
        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null)
            throw new ValidateException("Semester not found.");

        // Validate that the start date is before the end date
        if (semesterDto.StartDate >= semesterDto.EndDate)
            throw new ValidateException("StartDate must be before EndDate.");

        // Convert SemesterType string to enum
        if (!Enum.TryParse<SemesterType>(semesterDto.SemesterType.Replace(" ", ""), out var semesterType))
            throw new ValidateException("Invalid SemesterType specified.");

        // Update the semester details
        semester.SemesterType = semesterType;
        semester.StartDate = semesterDto.StartDate;
        semester.EndDate = semesterDto.EndDate;
        semester.AcademicYear = semesterDto.AcademicYear;

        await _context.SaveChangesAsync();

        return semester;
    }

    public async Task<bool> DeleteSemesterAsync(int semesterId)
    {
        // Find the semester by ID
        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null)
            return false; // Return false if semester is not found

        // Remove the semester from the database
        _context.Semesters.Remove(semester);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<SemesterDto>> GetAllSemestersAsync()
    {
        // Retrieve all semesters and map them to SemesterDto
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
        // Find the semester by ID
        var semester = await _context.Semesters.FindAsync(semesterId);

        // If the semester is not found, throw an error
        if (semester == null)
            throw new ValidateException("Semester not found.");

        // Return the semester details as a SemesterDto
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
            .Include(s => s.Class)
            .Select(s => new Student
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Address = s.Address,
                AcademicYear = s.AcademicYear,
                ParentName = s.ParentName,
                ParentEmail = s.ParentEmail, // Include ParentEmail
                Class = s.Class
            })
            .ToListAsync();
    }

    public async Task<Class> AddClassAsync(ClassDto newClassDto)
    {
        // Ensure the class name is not empty
        if (string.IsNullOrWhiteSpace(newClassDto.ClassName))
            throw new ArgumentException("Class name cannot be empty.");

        // Create a new class and save to the database
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
        // Find the existing class by ID
        var existingClass = await _context.Classes.FindAsync(classId);
        if (existingClass == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found.");

        // Update the class name
        existingClass.ClassName = updatedClassDto.ClassName;
        await _context.SaveChangesAsync();
        return existingClass;
    }

    public async Task<bool> DeleteClassAsync(int classId)
    {
        // Find the class to delete
        var classToDelete = await _context.Classes.FindAsync(classId);
        if (classToDelete == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found.");

        // Remove the class from the database
        _context.Classes.Remove(classToDelete);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Class>> GetAllClassesAsync()
    {
        // Retrieve all classes
        return await _context.Classes.ToListAsync();
    }

    public async Task<Class> GetClassByIdAsync(int classId)
    {
        // Find the class by ID
        var result = await _context.Classes.FindAsync(classId);
        if (result == null)
            throw new KeyNotFoundException($"Class with ID {classId} not found.");
        return result;
    }

    public async Task<bool> UpgradeClassAsync(int oldClassId, string oldAcademicYear, int newClassId,
        string newAcademicYear)
    {
        // Check if the new academic year exists
        var semesterExists = await _context.Semesters.AnyAsync(s => s.AcademicYear == newAcademicYear);
        if (!semesterExists)
        {
            throw new ValidateException("New academic year does not exist.");
        }

        // Retrieve student summaries for the old class and academic year
        var summaries = await _context.SummariesOfYear
            .Include(s => s.Student)
            .Where(s => s.Student.ClassId == oldClassId && s.AcademicYear == oldAcademicYear)
            .ToListAsync();

        // If no summaries are found, throw an error
        if (summaries.Count == 0)
        {
            throw new ValidateException("Class or Academic Year does not exist.");
        }

        // Update the class and academic year for the students
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

        // Save changes to the database
        await _context.SaveChangesAsync();
        return true;
    }

// Validate name format
    public bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidateException("Name cannot be empty.");

        var nameRegex = @"^[a-zA-Z\s]+$";
        if (!Regex.IsMatch(name, nameRegex))
            throw new ValidateException("Name contains invalid characters.");

        return true;
    }

// Validate email format
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidateException("Email cannot be empty.");

        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailRegex))
            throw new ValidateException("Invalid email format.");

        return true;
    }


    public async Task<List<AverageScore>> CalculateAndSaveAverageScoresForClassAsync(string className,
        string academicYear)
    {
        // Log to check the status of the calculation
        Console.WriteLine($"Starting to calculate scores for class {className} - Academic Year {academicYear}");

        // Fetch the class entity by class name
        var classEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassName == className);

        // If class not found, throw exception
        if (classEntity == null)
        {
            throw new Exception("Class not found");
        }

        // Fetch all students in the class for the specified academic year
        var studentsInClass = await _context.Students
            .Where(s => s.ClassId == classEntity.ClassId && s.AcademicYear == academicYear)
            .ToListAsync();

        var averageScoresList = new List<AverageScore>();

        // Loop through each student to calculate their average scores
        foreach (var student in studentsInClass)
        {
            Console.WriteLine($"Calculating scores for student: {student.StudentId}");

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

            // Convert decimal? values back to double? before assigning to AverageScore properties
            double? finalAverageSemester1 = averageSemester1.HasValue ? (double?)averageSemester1.Value : null;
            double? finalAverageSemester2 = averageSemester2.HasValue ? (double?)averageSemester2.Value : null;
            double? finalAverageAcademicYear = averageAcademicYear.HasValue ? (double?)averageAcademicYear.Value : null;

            // Find existing average score or create a new one if it doesn't exist
            var averageScore = await _context.AverageScores
                .FirstOrDefaultAsync(a => a.StudentId == student.StudentId && a.AcademicYear == academicYear);

            // If average score not found, add new record
            if (averageScore == null)
            {
                Console.WriteLine($"Adding new record for student {student.StudentId}");
                averageScore = new AverageScore
                {
                    StudentId = student.StudentId,
                    AverageSemester1 = finalAverageSemester1,
                    AverageSemester2 = finalAverageSemester2,
                    AverageAcademicYear = finalAverageAcademicYear,
                    AcademicYear = academicYear
                };
                _context.AverageScores.Add(averageScore);
            }
            else
            {
                // If record exists, update it
                Console.WriteLine($"Updating record for student {student.StudentId}");
                averageScore.AverageSemester1 = finalAverageSemester1;
                averageScore.AverageSemester2 = finalAverageSemester2;
                averageScore.AverageAcademicYear = finalAverageAcademicYear;
            }

            // Add the calculated average score to the result list
            averageScoresList.Add(averageScore);
        }

        // Save all changes to the database
        await _context.SaveChangesAsync();

        // Return the list of calculated and saved average scores
        return averageScoresList;
    }

    public async Task<IEnumerable<StudentScoreDto>> GetStudentAverageScoresAsync(int classId, string academicYear)
    {
        // Fetch all average scores for students in the specified class and academic year
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

    public async Task UpdateClassAndResetScoresAsync(string currentAcademicYear, string currentClassName,
        string newAcademicYear, string newClassName)
    {
        // Get new class ID by the new class name
        var newClassId = await _context.Classes
            .Where(c => c.ClassName == newClassName)
            .Select(c => c.ClassId)
            .FirstOrDefaultAsync();

        // If new class not found, throw exception
        if (newClassId == 0)
        {
            throw new ValidateException("New class does not exist.");
        }

        // Get list of students in the current class and academic year
        var studentsInCurrentClassAndYear = await _context.Students
            .Where(s => s.AcademicYear == currentAcademicYear && s.Class.ClassName == currentClassName)
            .Include(s => s.AverageScores)
            .Include(s => s.SubjectsAverageScores)
            .Include(s => s.Scores)
            .ToListAsync();

        // Loop through each student to check conditions and update their class or reset scores
        foreach (var student in studentsInCurrentClassAndYear)
        {
            var averageScore = student.AverageScores.FirstOrDefault();
            if (averageScore == null) continue;

            // Check if the student qualifies to move to the next class
            bool canMoveToNextClass = averageScore.AverageAcademicYear >= 6.5 &&
                                      averageScore.AverageSemester2 >= 6.5;

            // If the student can move to the next class, validate subject scores
            if (canMoveToNextClass)
            {
                bool subjectScoresValid = student.SubjectsAverageScores.All(sas =>
                    sas.SemesterAverage2 >= 5 && sas.AnnualAverage >= 5);

                // If subject scores are valid, update student information
                if (subjectScoresValid)
                {
                    student.ClassId = newClassId; // Update to new class
                    student.AcademicYear = newAcademicYear; // Update to new academic year
                    continue; // Move to the next student
                }
            }

            // If the student does not qualify, reset their scores and update the academic year
            // Remove scores from Score table
            if (student.Scores != null && student.Scores.Any())
            {
                _context.Scores.RemoveRange(student.Scores);
            }

            // Remove average score record
            if (averageScore != null)
            {
                _context.AverageScores.Remove(averageScore);
            }

            // Remove subject average scores
            if (student.SubjectsAverageScores != null && student.SubjectsAverageScores.Any())
            {
                _context.SubjectsAverageScores.RemoveRange(student.SubjectsAverageScores);
            }

            // Update the academic year while keeping the class unchanged
            student.AcademicYear = newAcademicYear;
        }

        // Save changes to the database
        await _context.SaveChangesAsync();
    }

// Custom exception class for validation errors
    public class ValidateException : Exception
    {
        public ValidateException(string message) : base(message)
        {
        }
    }
}