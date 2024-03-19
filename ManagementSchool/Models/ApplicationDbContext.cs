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
    public  DbSet<Class> Classes { get; set; }
    public DbSet<SchoolYear> SchoolYears { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedRoles(modelBuilder);
        
       // Configuration realtionship between Class 
        
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