using ManagementSchool.Entities;
using ManagementSchool.Service.RefreshToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManagementSchool.Models;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Class> Classes { get; set; }
    public DbSet<Grade> SchoolYears { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher?> Teachers { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<TeacherClass> TeacherClasses { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<StudentSubject> StudentSubjects { get; set; }
    public DbSet<ClassSubject> ClassSubjects { get; set; }
    public DbSet<ClassSemester> ClassSemesters { get; set; }
    public DbSet<Score> Scores { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<StudentSubjectScore> StudentSubjectScores { get; set; }
    public DbSet<Semester> Semesters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedRoles(modelBuilder);

        modelBuilder.Entity<Class>()
            .HasOne(sy => sy.Grade)
            .WithMany(c => c.Classes)
            .HasForeignKey(sy => sy.SchoolYearId);

        // Relationship with Student
        modelBuilder.Entity<Student>()
            .HasOne(c => c.Class)
            .WithMany(s => s.Students)
            .HasForeignKey(c => c.ClassId);

        // Relationship with TeacherClass
        modelBuilder.Entity<TeacherClass>()
            .HasOne(c => c.Class)
            .WithMany(tc => tc.TeacherClasses)
            .HasForeignKey(c => c.ClassId);

        // Relationship with ClassSubject
        modelBuilder.Entity<ClassSubject>()
            .HasOne(c => c.Class)
            .WithMany(cs => cs.ClassSubjects)
            .HasForeignKey(c => c.ClassId);

        // Relationship with Parent
        modelBuilder.Entity<Student>()
            .HasOne(p => p.Parent)
            .WithMany(s => s.Students)
            .HasForeignKey(p => p.ParentId);

        // Relationship with StudentSubject
        modelBuilder.Entity<StudentSubject>()
            .HasOne(s => s.Student)
            .WithMany(ss => ss.StudentSubjects)
            .HasForeignKey(s => s.StudentId);
        // Relationship with Teacher
        modelBuilder.Entity<Teacher>()
            .HasOne(s => s.Subject)
            .WithMany(t => t.Teachers)
            .HasForeignKey(s => s.SubjectId);

        // Relationship with StudentSubject
        modelBuilder.Entity<StudentSubject>()
            .HasOne(s => s.Subject)
            .WithMany(ss => ss.StudentSubjects)
            .HasForeignKey(s => s.SubjectId);

        // Relationship with ClassSubject
        modelBuilder.Entity<ClassSubject>()
            .HasOne(s => s.Subject)
            .WithMany(cs => cs.ClassSubjects)
            .HasForeignKey(s => s.SubjectId);

        modelBuilder.Entity<TeacherClass>()
            .HasOne(t => t.Teacher)
            .WithMany(tc => tc.TeacherClasses)
            .HasForeignKey(t => t.TeacherId);

        // Cấu hình mối quan hệ nhiều-nhiều giữa Class và Semester thông qua ClassSemester
        modelBuilder.Entity<ClassSemester>()
            .HasKey(cs => new { cs.ClassId, cs.SemesterId });

        modelBuilder.Entity<ClassSemester>()
            .HasOne(cs => cs.Class)
            .WithMany(c => c.ClassSemesters)
            .HasForeignKey(cs => cs.ClassId);

        modelBuilder.Entity<ClassSemester>()
            .HasOne(cs => cs.Semester)
            .WithMany(s => s.ClassSemesters)
            .HasForeignKey(cs => cs.SemesterId);
        // Cấu hình quan hệ Score với Student
        modelBuilder.Entity<Score>()
            .HasOne(s => s.Student)
            .WithMany(st => st.Scores)
            .HasForeignKey(s => s.StudentId);

        // Cấu hình quan hệ Score với Subject
        modelBuilder.Entity<Score>()
            .HasOne(s => s.Subject)
            .WithMany(sub => sub.Scores)
            .HasForeignKey(s => s.SubjectId);

        // Thiết lập mối quan hệ giữa StudentSubjectScore và Student
        modelBuilder.Entity<StudentSubjectScore>()
            .HasOne(sss => sss.Student)
            .WithMany(s => s.StudentSubjectScores)
            .HasForeignKey(sss => sss.StudentId);

// Thiết lập mối quan hệ giữa StudentSubjectScore và Subject
        modelBuilder.Entity<StudentSubjectScore>()
            .HasOne(sss => sss.Subject)
            .WithMany(sub => sub.StudentSubjectScores)
            .HasForeignKey(sss => sss.SubjectId);


        // Seed date for grade
        modelBuilder.Entity<Grade>().HasData(
            new Grade { GradeId = 1, YearName = "10" },
            new Grade { GradeId = 2, YearName = "11" },
            new Grade { GradeId = 3, YearName = "12" }
        );

        // Seed data for class, one grade have 13 class
        for (var year = 1; year <= 3; year++)
        for (var classNum = 1; classNum <= 13; classNum++)
            modelBuilder.Entity<Class>().HasData(
                new Class
                {
                    ClassId = year * 100 + classNum,
                    ClassName = $"{year + 9}/{classNum}", // This will result in "10/1", "10/2", ..., "12/13"
                    SchoolYearId = year
                }
            );

        // Seed data for Subject 
        modelBuilder.Entity<Subject>().HasData(
            new Subject { SubjectId = 1, SubjectName = "Math" },
            new Subject { SubjectId = 2, SubjectName = "Literature" },
            new Subject { SubjectId = 3, SubjectName = "English" },
            new Subject { SubjectId = 4, SubjectName = "Physics" },
            new Subject { SubjectId = 5, SubjectName = "Chemistry" },
            new Subject { SubjectId = 6, SubjectName = "Biology" },
            new Subject { SubjectId = 7, SubjectName = "History" },
            new Subject { SubjectId = 8, SubjectName = "Geography" },
            new Subject { SubjectId = 9, SubjectName = "Civics" },
            new Subject { SubjectId = 10, SubjectName = "Computer Science" },
            new Subject { SubjectId = 11, SubjectName = "Sport" },
            new Subject { SubjectId = 12, SubjectName = "Defense Education" }
        );
    }

    private void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
            new IdentityRole { Name = "Student", ConcurrencyStamp = "2", NormalizedName = "STUDENT" },
            new IdentityRole { Name = "Teacher", ConcurrencyStamp = "3", NormalizedName = "TEACHER" },
            new IdentityRole { Name = "Parent", ConcurrencyStamp = "4", NormalizedName = "PARENT" }
        );
    }
}