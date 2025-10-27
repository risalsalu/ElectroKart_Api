using ElectroKart_Api.Models;
using System.Security.Claims;

namespace ElectroKart_Api.Services.Auth
{
    public interface IJWTGenerator
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    }
}
