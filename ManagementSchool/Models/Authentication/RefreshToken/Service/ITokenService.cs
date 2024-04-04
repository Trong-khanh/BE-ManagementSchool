using Microsoft.AspNetCore.Identity;

namespace ManagementSchool.Models.Authentication.RefreshToken.Service;

public interface ITokenService
{
    Task<RefreshToken> GenerateRefreshToken(string ipAddress, IdentityUser user);
}