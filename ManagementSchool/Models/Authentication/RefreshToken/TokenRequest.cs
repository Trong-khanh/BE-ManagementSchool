namespace ManagementSchool.Models.Authentication.RefreshToken;

public class TokenRequest
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}