using System.ComponentModel.DataAnnotations;

namespace ManagementSchool.Entities;

public enum SemesterType
{
    [Display(Name = "Semester 1")] Semester1,

    [Display(Name = "Semester 2")] Semester2
}