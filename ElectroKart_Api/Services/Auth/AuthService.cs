using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Auth;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ElectroKart_Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJWTGenerator _jwtGenerator;

        public AuthService(IAuthRepository userRepository, IPasswordHasher<User> passwordHasher, IJWTGenerator jwtGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<User?> Register(RegisterDTO dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null) return null;

            var user = new User { Username = dto.Username, Email = dto.Email, Role = "User" };
            user.Password = _passwordHasher.HashPassword(user, dto.Password);
            await _userRepository.AddUserAsync(user);
            return user;
        }

        public async Task<LoginResponseDto?> Login(LoginDTO dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null) return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (verificationResult == PasswordVerificationResult.Success)
            {
                var accessToken = _jwtGenerator.GenerateToken(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userRepository.UpdateUserAsync(user);

                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }

            return null;
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(TokenRequestDto tokenRequestDto)
        {
            var principal = _jwtGenerator.GetPrincipalFromExpiredToken(tokenRequestDto.AccessToken);
            var userIdString = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return null;
            }

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user is null || user.RefreshToken != tokenRequestDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            var newAccessToken = _jwtGenerator.GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateUserAsync(user);

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}