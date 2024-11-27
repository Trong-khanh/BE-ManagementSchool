using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ManagementSchool.Dto;
using ManagementSchool.Models;
using ManagementSchool.Models.Login;
using ManagementSchool.Models.SignUp;
using ManagementSchool.Service.RefreshToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using User.ManagementSchool.Service.Models;
using User.ManagementSchool.Service.Service;

namespace ManagementSchool.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthenticateController> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthenticateController(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager, IEmailService emailService, IConfiguration configuration,
        SignInManager<IdentityUser> signInManager,
        TokenService tokenService, ILogger<AuthenticateController> logger, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _configuration = configuration;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
        _context = context;
    }

 
    [HttpPost("Regsiter") ]
    public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
    {
        // Check if user already exists
        var userExist = await _userManager.FindByNameAsync(registerUser.Email);
        if (userExist != null)
            return StatusCode(StatusCodes.Status403Forbidden,
                new Response { Status = "Error", Message = "User already exists." });

        // Check if the role is 'Admin' and ensure only one admin exists
        if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (admins.Count > 0)
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response
                    {
                        Status = "Error",
                        Message = "An admin account already exists. No additional admin accounts can be created."
                    });
        }

        // Proceed if role exists
        if (await _roleManager.RoleExistsAsync(role))
        {
            var user = new IdentityUser
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.UserName
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User creation failed." });

            // Add role to user
            await _userManager.AddToRoleAsync(user, role);

            // Generate and send email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authenticate",
                new { token, email = user.Email }, Request.Scheme);

            // Nội dung HTML cho email
            var confirmationHtml = $@"
        <html>
        <head>
            <style>
                .btn {{
                    background-color: #4CAF50;
                    border: none;
                    color: white;
                    padding: 10px 20px;
                    text-align: center;
                    text-decoration: none;
                    display: inline-block;
                    font-size: 16px;
                    margin: 4px 2px;
                    cursor: pointer;
                    border-radius: 5px;
                }}
            </style>
        </head>
        <body>
            <h2>Welcome to Management School!</h2>
            <p>Please confirm your email address by clicking the button below:</p>
            <a href='{confirmationLink}' class='btn'>Confirm Email</a>
        </body>
        </html>";

            // Tạo và gửi email
            var message = new Message(new[] { user.Email }, "Email Confirmation", confirmationHtml, isHtml: true);
            _emailService.SendEmailAsync(message);

            return StatusCode(StatusCodes.Status200OK,
                new Response
                {
                    Status = "Success",
                    Message = $"User created and email confirmation sent to {user.Email} successfully."
                });
        }

        return StatusCode(StatusCodes.Status500InternalServerError,
            new Response { Status = "Error", Message = "Role does not exist." });
    }


[HttpGet("ConfirmEmail")]
public async Task<IActionResult> ConfirmEmail(string token, string email)
{
    var user = await _userManager.FindByEmailAsync(email);
    
    // Nếu tìm thấy người dùng
    if (user != null)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);
        
        // Nếu xác nhận email thành công
        if (result.Succeeded)
        {
            var confirmationHtml = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        text-align: center;
                        background-color: #f4f4f4;
                        padding: 50px;
                    }}
                    .message {{
                        background-color: #4CAF50;
                        color: white;
                        padding: 15px;
                        border-radius: 5px;
                        display: inline-block;
                        margin-top: 20px;
                    }}
                    h2 {{
                        color: #333;
                    }}
                    a {{
                        color: #4CAF50;
                        text-decoration: none;
                        font-weight: bold;
                        margin-top: 20px;
                        display: inline-block;
                    }}
                    a:hover {{
                        color: #45a049;
                    }}
                </style>
            </head>
            <body>
                <h2>Email Verification Successful!</h2>
                <div class='message'>
                    Your email has been successfully verified. You can now log in to your account.
                </div>
                <a href='http://localhost:3000/login'>Go to Login</a>
            </body>
            </html>";

            return Content(confirmationHtml, "text/html");
        }
    }

    // Nếu không tìm thấy người dùng hoặc xác nhận không thành công
    var errorHtml = $@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                text-align: center;
                background-color: #f4f4f4;
                padding: 50px;
            }}
            .message {{
                background-color: #f44336;
                color: white;
                padding: 15px;
                border-radius: 5px;
                display: inline-block;
                margin-top: 20px;
            }}
            h2 {{
                color: #333;
            }}
        </style>
    </head>
    <body>
        <h2>Email Verification Failed!</h2>
        <div class='message'>
            We could not verify your email. Please try again or contact support.
        </div>
    </body>
    </html>";

    return Content(errorHtml, "text/html");
}



    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _logger.LogInformation("Attempting login for {User}", loginUser.UserName);

        var user = await _userManager.FindByNameAsync(loginUser.UserName);
        if (user == null)
        {
            _logger.LogWarning("Login failed for {User}: User not found", loginUser.UserName);
            return Unauthorized(new { message = "Invalid login attempt." });
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginUser.Password, loginUser.RememberMe, false);
        if (result.Succeeded)
        {
            _logger.LogInformation("Signin succeeded for {User}", loginUser.UserName);
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id, Guid.NewGuid().ToString());
            await SaveRefreshTokenAsync(refreshToken);

            var userRole = roles.FirstOrDefault() ?? "User";
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = JsonConvert.SerializeObject(user),
                Role = userRole
            });
        }

        _logger.LogWarning("Signin failed for {User}.", loginUser.UserName);
        return Unauthorized(new { message = "Username or password is incorrect" });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
    {
        var refreshToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.Token == request.Token && !rt.IsRevoked);
        if (refreshToken == null) return BadRequest(new { message = "Invalid refresh token." });
        if (refreshToken.ExpiresUtc < DateTime.UtcNow)
            return Unauthorized(new { message = "Refresh token has expired. Please log in again." });
        var user = await _userManager.FindByIdAsync(refreshToken.UserId);
        if (user == null) return BadRequest(new { message = "User not found." });
        var newAccessToken = await _tokenService.GenerateAccessToken(user);
        _logger.LogInformation("Return new access token");

        return Ok(new
        {
            AccessToken = newAccessToken
        });
    }

    private async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
    {
        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == refreshToken.UserId && !rt.IsRevoked);

        if (existingToken != null)
        {
            existingToken.Token = refreshToken.Token;
            existingToken.IssuedUtc = refreshToken.IssuedUtc;
            existingToken.ExpiresUtc = refreshToken.ExpiresUtc;
            existingToken.JwtId = refreshToken.JwtId;
            existingToken.IsRevoked = refreshToken.IsRevoked;
            existingToken.ReplacedByToken = null;
        }
        else
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        await _context.SaveChangesAsync();
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddMinutes(30),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
        return token;
    }
}