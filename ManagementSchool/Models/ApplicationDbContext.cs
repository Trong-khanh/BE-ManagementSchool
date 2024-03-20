using ManagementSchool.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManagementSchool.Models;

public class ApplicationDbContext: IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options): base(options)
    {
    }
    public DbSet<Class> Classes { get; set; }
    public DbSet<SchoolYear> SchoolYears { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<TeacherClass> TeacherClasses { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedRoles(modelBuilder);
        
       // Configuration realtionship between Class 
       // Configure the primary keys
       modelBuilder.Entity<SchoolYear>().HasKey(sy => sy.SchoolYearId);
       modelBuilder.Entity<Class>().HasKey(c => c.ClassId);
       modelBuilder.Entity<Parent>().HasKey(p => p.ParentId);
       modelBuilder.Entity<Student>().HasKey(s => s.StudentId);
       modelBuilder.Entity<Teacher>().HasKey(t => t.TeacherId);
       modelBuilder.Entity<TeacherClass>().HasKey(tc => new { tc.TeacherId, tc.ClassId });

       // Configure the relationships
       modelBuilder.Entity<Class>()
           .HasOne(c => c.SchoolYear)
           .WithMany(sy => sy.Classes)
           .HasForeignKey(c => c.SchoolYearId);

       modelBuilder.Entity<Student>()
           .HasOne(s => s.Class)
           .WithMany(c => c.Students)
           .HasForeignKey(s => s.ClassId);

       modelBuilder.Entity<Student>()
           .HasOne(s => s.Parent)
           .WithMany(p => p.Students)
           .HasForeignKey(s => s.ParentId);

       modelBuilder.Entity<TeacherClass>()
           .HasOne(tc => tc.Teacher)
           .WithMany(t => t.TeacherClasses)
           .HasForeignKey(tc => tc.TeacherId);

       modelBuilder.Entity<TeacherClass>()
           .HasOne(tc => tc.Class)
           .WithMany(c => c.TeacherClasses)
           .HasForeignKey(tc => tc.ClassId);
        // Seed date for grade
        modelBuilder.Entity<SchoolYear>().HasData(
            new SchoolYear { SchoolYearId = 1, Name = "10" },
            new SchoolYear { SchoolYearId = 2, Name = "11" },
            new SchoolYear { SchoolYearId = 3, Name = "12" }
        );
        
// Seed data for class, one grade have 13 class
        for (int year = 1; year <= 3; year++)
        {
            for (int classNum = 1; classNum <= 13; classNum++)
            {
                modelBuilder.Entity<Class>().HasData(
                    new Class 
                    { 
                        ClassId = year * 100 + classNum, 
                        Name = $"{year + 9}/{classNum}", // This will result in "10/1", "10/2", ..., "12/13"
                        SchoolYearId = year 
                    }
                );
            }
        }
        
    }
    
    

    private  void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole(){ Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
            new IdentityRole(){ Name = "Student", ConcurrencyStamp = "2", NormalizedName = "STUDENT" },
            new IdentityRole(){ Name = "Teacher", ConcurrencyStamp = "3", NormalizedName = "TEACHER" }, 
            new IdentityRole(){ Name = "Parent", ConcurrencyStamp = "4", NormalizedName = "PARENT" }
        );
    }
}