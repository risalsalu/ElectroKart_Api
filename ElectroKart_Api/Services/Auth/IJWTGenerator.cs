using ElectroKart_Api.Models;

namespace ElectroKart_Api.Services.Auth
{
    public interface IJWTGenerator
    {
        string GenerateToken(User user);
    }
}