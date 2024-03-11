using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManagementSchool.Models;

public class ApplicationDbContext: IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        SeedRoles(builder);
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