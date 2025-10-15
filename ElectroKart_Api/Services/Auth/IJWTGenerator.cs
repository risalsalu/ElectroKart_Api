using ElectroKart_Api.Models;
using System.Security.Claims;

namespace ElectroKart_Api.Services.Auth
{
    public interface IJWTGenerator
    {
        string GenerateToken(User user);

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    }
}