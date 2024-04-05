using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ManagementSchool.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation("JwtMiddleware: Processing request...");
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            try
            {
                _logger.LogInformation("Token found");

                await AttachUserToContext(context, token);
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogInformation("Token Expired ");
                context.Response.StatusCode = 401;
                context.Response.Headers.Append("Token-Expired", "true");
                return;
            }

        await _next(context);
    }

    private Task AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.ReadToken(token) is JwtSecurityToken jwtToken)
            {
                var userId = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                var roles = jwtToken.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                context.Items["UserId"] = userId;
                context.Items["Roles"] = roles;

                _logger.LogInformation(
                    $"JwtMiddleware: User {userId} with roles {string.Join(", ", roles)} attached to context.");
            }
            else
            {
                _logger.LogWarning("JwtMiddleware: Failed to parse token or token is null.");
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "JwtMiddleware: Error in AttachUserToContext.");
        }

        return Task.CompletedTask;
    }
}