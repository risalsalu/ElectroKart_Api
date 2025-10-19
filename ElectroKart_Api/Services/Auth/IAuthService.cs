﻿using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Auth;
using ElectroKart_Api.Models;

namespace ElectroKart_Api.Services.Auth
{
    public interface IAuthService
    {
        Task<User?> Register(RegisterDTO dto);
        Task<LoginResponseDto?> Login(LoginDTO dto);
        Task<List<User>> GetAllUsers();
    }
}
