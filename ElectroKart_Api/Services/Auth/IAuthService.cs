using ElectroKart_Api.DTOs;
using ElectroKart_Api.Models;
using System.Collections.Generic;

namespace ElectroKart_Api.Services.Auth
{
    public interface IAuthService
    {
        User? Register(RegisterDTO dto);      // null if registration failed (e.g. duplicate)
        User? Login(LoginDTO dto);            // null if invalid credentials
        List<User> GetAllUsers();
    }
}
