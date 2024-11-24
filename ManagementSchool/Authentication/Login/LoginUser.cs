using System.ComponentModel.DataAnnotations;

namespace ManagementSchool.Models.Login;

public class LoginUser
{
    [Required(ErrorMessage = "User Name is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}