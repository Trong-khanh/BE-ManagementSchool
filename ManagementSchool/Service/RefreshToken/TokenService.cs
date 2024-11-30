using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ManagementSchool.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ManagementSchool.Service.RefreshToken;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TokenService> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public TokenService(IConfiguration configuration, UserManager<IdentityUser> userManager,
        ApplicationDbContext context, ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public async Task<string> GenerateAccessToken(IdentityUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddHours(1);

        // Lấy claims và roles của người dùng
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        // Tạo claims cho roles
        var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

        // Tạo list claims và thêm email như một claim mới
        var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id),
                // Thêm email như một claim
                new(ClaimTypes.Email, user.Email)
            }
            .Union(userClaims)
            .Union(roleClaims);

        // Tạo token JWT
        var token = new JwtSecurityToken(
            _configuration["Jwt:validIssuer"],
            _configuration["Jwt:validAudience"],
            claims,
            expires: expiry,
            signingCredentials: creds
        );

        // Trả về token dưới dạng chuỗi
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(string userId, string jwtId)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = Guid.NewGuid().ToString().Replace("-", ""),
            IssuedUtc = DateTime.UtcNow,
            ExpiresUtc = DateTime.UtcNow.AddDays(7),
            JwtId = jwtId,
            IsRevoked = false
        };
    }
}