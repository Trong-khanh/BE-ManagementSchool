using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ManagementSchool.Models;
using ManagementSchool.Models.Authentication.RefreshToken;
using ManagementSchool.Models.Authentication.RefreshToken.Service;
using ManagementSchool.Models.Login;
using ManagementSchool.Models.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using User.ManagementSchool.Service.Models;
using User.ManagementSchool.Service.Service;

namespace ManagementSchool.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public AuthenticateController(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager, IEmailService emailService, IConfiguration configuration,
        SignInManager<IdentityUser> signInManager, ApplicationDbContext context, ITokenService tokenService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _emailService = emailService;
        _configuration = configuration;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
    {
        // check user exist or not
        var userExsit = await _userManager.FindByNameAsync(registerUser.Email);
        if (userExsit != null)
            return StatusCode(StatusCodes.Status403Forbidden,
                new Response { Status = "Error", Message = "User already exist" });
        // Add the user in db
        IdentityUser user = new()
        {
            Email = registerUser.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerUser.UserName,
            TwoFactorEnabled = true
        };

        if (await _roleManager.RoleExistsAsync(role))
        {
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User failed to create" });

            //Add roel to user
            await _userManager.AddToRoleAsync(user, role);
            // Add token to verify email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationslink = Url.Action(nameof(ConfirmEmail), "Authenticate",
                new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email }, "Email Confirmation", confirmationslink);
            _emailService.SendEmail(message);

            return StatusCode(StatusCodes.Status200OK,
                new Response
                    { Status = "Success", Message = $"User created & Email Sent To {user.Email} Successfully" });
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "Role does not exist" });
        }
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = "Email Verified Successfully" });
        }

        return StatusCode(StatusCodes.Status500InternalServerError,
            new Response { Status = "Error", Message = "This User Doesnot Exist" });
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
    {
        var user = await _userManager.FindByNameAsync(loginUser.UserName);
        if (user.TwoFactorEnabled)
        {
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(user, loginUser.Password, false, false);
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var message = new Message(new string[] { user.Email }, "OTP Confirmation", token);
            _emailService.SendEmail(message);

            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = $"We have sent an OTP email to {user.Email}" });
        }

        if (user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password))
        {
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles) authClaims.Add(new Claim(ClaimTypes.Role, role));

            var jwtToken = GetToken(authClaims);
            var refreshToken = await _tokenService.GenerateRefreshToken(HttpContext.Connection.RemoteIpAddress.ToString(), user);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo,
                refreshToken = refreshToken.Token,
                refreshTokenExpiration = refreshToken.Expires
            });
        }

        return Unauthorized();
    }

    [HttpPost]
    [Route("Login- 2FA")]
    public async Task<IActionResult> LoginWithOTP(string code, string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        var SignIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
        if (SignIn.Succeeded)
            if (user != null)
            {
                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles) authClaims.Add(new Claim(ClaimTypes.Role, role));

                var jwtToken = GetToken(authClaims);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });
            }

        return StatusCode(StatusCodes.Status403Forbidden,
            new Response() { Status = "Error", Message = "Invalid OTP" });
    }
    

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.UtcNow.AddMinutes(3), 
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }

    
    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        var refreshToken = _context.RefreshTokens.SingleOrDefault(rt => rt.Token == tokenRequest.RefreshToken && rt.IsActive);

        if (refreshToken == null)
        {
            return Unauthorized("Invalid refresh token");
        }

        var user = await _userManager.FindByIdAsync(refreshToken.UserId);
        if (user == null)
        {
            return Unauthorized("Invalid refresh token");
        }

        var newAccessToken = GetToken(new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        });

        var newRefreshToken = await _tokenService.GenerateRefreshToken(HttpContext.Connection.RemoteIpAddress.ToString(), user);
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = HttpContext.Connection.RemoteIpAddress.ToString();
        refreshToken.ReplacedByToken = newRefreshToken.Token;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            Expiration = newAccessToken.ValidTo,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.Expires
        });
    }

    
    private async Task<RefreshToken> GenerateRefreshToken(string ipAddress, IdentityUser user)
    {
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

}