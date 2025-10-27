using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Auth;
using ElectroKart_Api.Exceptions;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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

        public async Task<User> Register(RegisterDTO dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new BadRequestException("Email is already registered.");

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

        public async Task<LoginResponseDto> Login(LoginDTO dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                throw new NotFoundException("User not found.");

            var verify = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (verify != PasswordVerificationResult.Success)
                throw new UnauthorizedException("Invalid credentials.");

            if (user.IsBlocked)
                throw new UnauthorizedException("Your account has been blocked.");

            var accessToken = _jwtGenerator.GenerateToken(user);
            var refreshToken = _jwtGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateUserAsync(user);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsBlocked = user.IsBlocked
            };
        }

        public async Task<LoginResponseDto> RefreshToken(string expiredToken, string refreshToken)
        {
            var principal = _jwtGenerator.GetPrincipalFromExpiredToken(expiredToken);
            var email = principal?.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                throw new UnauthorizedException("Invalid token.");

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedException("Invalid refresh token.");

            var newAccessToken = _jwtGenerator.GenerateToken(user);
            var newRefreshToken = _jwtGenerator.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateUserAsync(user);

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
