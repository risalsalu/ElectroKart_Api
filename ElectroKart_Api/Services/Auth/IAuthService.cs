using ElectroKart_Api.DTOs;
using ElectroKart_Api.Models;

namespace ElectroKart_Api.Services.Auth
{
    public interface IAuthService
    {
        Task<User?> Register(RegisterDTO dto);
        Task<(User? user, string? token)> Login(LoginDTO dto);
        Task<List<User>> GetAllUsers();
    }
}