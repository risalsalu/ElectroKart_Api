using ElectroKart_Api.DTOs;
using ElectroKart_Api.Models;

namespace ElectroKart_Api.Services
{
    public interface IUserService
    {
        User Register(RegisterDTO Dto);
        User? Login(LoginDTO Dto);
        List <User> GetAllUsers();
    }
}
