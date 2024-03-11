
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
            // Add token to verify email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationslink = Url.Action(nameof(ConfirmEmail), "Authenticate", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email }, "Email Confirmation", confirmationslink);
            _emailService.SendEmail(message);
            
            return StatusCode(StatusCodes.Status200OK, 
                new Response { Status ="Success", Message = $"User created & Email Sent To {user.Email} Successfully" });
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new Response { Status ="Error", Message = "Role does not exist" });
        }
    }

    [HttpGet("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
          var result = await _userManager.ConfirmEmailAsync(user, token);
          if (!result.Succeeded)
          {
              return StatusCode(StatusCodes.Status200OK,
                  new Response { Status ="Success", Message = "Email Verified Successfully" });
          }
        }
        return StatusCode(StatusCodes.Status500InternalServerError,
            new Response{Status = "Error", Message = "This User Doesnot Exist"});
    }
}