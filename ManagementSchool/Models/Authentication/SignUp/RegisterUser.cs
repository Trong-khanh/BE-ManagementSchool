using System.ComponentModel.DataAnnotations;

namespace ManagementSchool.Models.SignUp;

public class RegisterUser
{
    [Required(ErrorMessage = "User name is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
}