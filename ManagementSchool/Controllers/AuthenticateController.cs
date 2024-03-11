
using ManagementSchool.Models.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User.ManagementSchool.Service.Models;
using User.ManagementSchool.Service.Service;

namespace ManagementSchool.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthenticateController: ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailService _emailService;
    
    public AuthenticateController(UserManager<IdentityUser> userManager,
        RoleManager< IdentityRole> roleManager, IEmailService emailService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _emailService = emailService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role )
    {
        // check user exist or not
        var userExsit = await _userManager.FindByNameAsync(registerUser.Email);
        if (userExsit != null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, 
                new Response { Status ="Error", Message = "User already exist" });
        }
        // Add the user in db
        IdentityUser user = new()
        { 
            Email = registerUser.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerUser.UserName
        };

        if (await _roleManager.RoleExistsAsync(role))
        {
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new Response { Status ="Error", Message = "User failed to create" });
            }

            //Add roel to user
            await _userManager.AddToRoleAsync(user, role);
            return StatusCode(StatusCodes.Status200OK, 
                new Response { Status ="Success", Message = "User create success" });
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new Response { Status ="Error", Message = "Role does not exist" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> TestEmail()
    {
        var message = new Message(new string[] { "tltkhanh1501@gmail.com" }, "Test Email", " <h1>This is a test email</h1>");
        _emailService.SendEmail(message);
        return StatusCode(StatusCodes.Status200OK,
        new Response{Status = "Success", Message = "Email sent"});
    }
}