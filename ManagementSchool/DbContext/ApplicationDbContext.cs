using ManagementSchool.Entities;
using ManagementSchool.Service.RefreshToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayPal.v1.Invoices;

namespace ManagementSchool.Models;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Class> Classes { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher?> Teachers { get; set; }
    public DbSet<TeacherClass> TeacherClasses { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<StudentSubject> StudentSubjects { get; set; }
    public DbSet<ClassSubject> ClassSubjects { get; set; }
    public DbSet<ClassSemester> ClassSemesters { get; set; }
    public DbSet<Score> Scores { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<Semester> Semesters { get; set; }
    public DbSet<SummaryOfYear> SummariesOfYear { get; set; }
    public DbSet<SubjectsAverageScore> SubjectsAverageScores { get; set; }
    public DbSet<AverageScore> AverageScores { get; set; }
    public DbSet<TuitionFeeNotification> TuitionFeeNotifications { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedRoles(modelBuilder);
        
        // Thiết lập mối quan hệ một - một giữa Semester và TuitionFeeNotification
        modelBuilder.Entity<TuitionFeeNotification>()
            .HasOne(t => t.Semester)  
            .WithOne(s => s.TuitionFeeNotification) 
            .HasForeignKey<TuitionFeeNotification>(t => t.SemesterId)  
            .IsRequired();  
        
        modelBuilder.Entity<TuitionFeeNotification>()
            .HasIndex(t => t.SemesterId) 
            .IsUnique();  
        
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

        // Relationship with StudentSubject
        modelBuilder.Entity<StudentSubject>()
            .HasOne(s => s.Student)
            .WithMany(ss => ss.StudentSubjects)
            .HasForeignKey(s => s.StudentId);

        modelBuilder.Entity<StudentSubject>()
            .HasOne(s => s.Subject)
            .WithMany(ss => ss.StudentSubjects)
            .HasForeignKey(s => s.SubjectId);

        // Relationship with Teacher
        modelBuilder.Entity<Teacher>()
            .HasOne(s => s.Subject)
            .WithMany(t => t.Teachers)
            .HasForeignKey(s => s.SubjectId);

        modelBuilder.Entity<TeacherClass>()
            .HasOne(t => t.Teacher)
            .WithMany(tc => tc.TeacherClasses)
            .HasForeignKey(t => t.TeacherId);

        // Many-to-many relationship between Class and Semester
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

        // Relationships for Score
        modelBuilder.Entity<Score>()
            .HasOne(s => s.Student)
            .WithMany(st => st.Scores)
            .HasForeignKey(s => s.StudentId);

        modelBuilder.Entity<Score>()
            .HasOne(s => s.Subject)
            .WithMany(sub => sub.Scores)
            .HasForeignKey(s => s.SubjectId);

        modelBuilder.Entity<Score>()
            .HasOne(s => s.Semester)
            .WithMany(s => s.Scores)
            .HasForeignKey(s => s.SemesterId);
        
        // modelBuilder.Entity<Score>()
        //     .HasOne(s => s.SubjectsAverageScore)
        //     .WithMany(sa => sa.Scores)
        //     .HasForeignKey(s => s.SubjectsAverageScoreId)
        //     .OnDelete(DeleteBehavior.Restrict);

        // Relationship between Student and SummaryOfYear
        modelBuilder.Entity<SummaryOfYear>()
            .HasOne(sy => sy.Student)
            .WithMany(s => s.SummariesOfYear);

        // Configuring enum conversion for SemesterType
        modelBuilder.Entity<Semester>()
            .Property(s => s.SemesterType)
            .HasConversion(
                v => v.ToString(),
                v => (SemesterType)Enum.Parse(typeof(SemesterType), v));

        // Configuring enum conversion for ExamType
        modelBuilder.Entity<Score>()
            .Property(s => s.ExamType)
            .HasConversion(
                v => v.ToString(),
                v => (ExamType)Enum.Parse(typeof(ExamType), v));

        // Configure relationships for SubjectsAverageScore
        modelBuilder.Entity<SubjectsAverageScore>()
            .HasOne(s => s.Student)
            .WithMany(a => a.SubjectsAverageScores)
            .HasForeignKey(s => s.StudentId);

        modelBuilder.Entity<SubjectsAverageScore>()
            .HasOne(s => s.Semester)
            .WithMany(a => a.AverageScores)
            .HasForeignKey(s => s.SemesterId);

        modelBuilder.Entity<SubjectsAverageScore>()
            .HasOne(s => s.Subject)
            .WithMany(a => a.AverageScores)
            .HasForeignKey(s => s.SubjectId);

        // Thiết lập quan hệ giữa Student và AverageScore
        modelBuilder.Entity<AverageScore>()
            .HasOne(a => a.Student)
            .WithMany(s => s.AverageScores)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

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