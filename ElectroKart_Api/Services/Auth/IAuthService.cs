using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Auth;
using ElectroKart_Api.Models;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Auth
{
    public interface IAuthService
    {
        Task<User> Register(RegisterDTO dto);
        Task<LoginResponseDto> Login(LoginDTO dto);
        Task<LoginResponseDto> RefreshToken(string expiredToken, string refreshToken);
    }
}
