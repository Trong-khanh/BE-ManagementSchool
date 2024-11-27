using System.ComponentModel.DataAnnotations;

public enum ExamType
{
    [Display(Name = "Test When Class Begins")]
    TestWhenClassBegins = 0, // Gán giá trị 0

    [Display(Name = "15 Minutes Test")] FifteenMinutesTest = 1, // Gán giá trị 1

    [Display(Name = "45 Minutes Test")] FortyFiveMinutesTest = 2, // Gán giá trị 2

    [Display(Name = "Semester Test")] SemesterTest = 3 // Gán giá trị 3
}