﻿using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Auth;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Auth;
using Microsoft.AspNetCore.Identity;

namespace ElectroKart_Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJWTGenerator _jwtGenerator;

        public AuthService(
            IAuthRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IJWTGenerator jwtGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
        }

        // Register new user
        public async Task<User?> Register(RegisterDTO dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null) return null;

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = "User"
            };

            user.Password = _passwordHasher.HashPassword(user, dto.Password);
            await _userRepository.AddUserAsync(user);
            return user;
        }

        // Login
        public async Task<LoginResponseDto?> Login(LoginDTO dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null) return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (verificationResult != PasswordVerificationResult.Success)
                return null;

            if (user.IsBlocked)
            {
                return new LoginResponseDto
                {
                    ErrorMessage = "Your account has been blocked. Please contact support.",
                    IsBlocked = true
                };
            }

            var accessToken = _jwtGenerator.GenerateToken(user);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsBlocked = user.IsBlocked
            };
        }
    }
}
