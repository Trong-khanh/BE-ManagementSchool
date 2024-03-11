
using ManagementSchool.Models.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthenticateController: ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    
    public AuthenticateController(UserManager<IdentityUser> userManager,
        RoleManager< IdentityRole> roleManager, IConfiguration configuration)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
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
}