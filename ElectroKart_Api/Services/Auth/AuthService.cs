using ElectroKart_Api.Data;
using ElectroKart_Api.DTOs;
using ElectroKart_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElectroKart_Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public User? Register(RegisterDTO dto)
        {
            // Check duplicate email
            if (_context.Users.Any(u => u.Email == dto.Email))
            {
                return null; // caller should handle (e.g. return BadRequest)
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                // Password will be replaced with hash below
            };

            user.Password = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            // Do not return the password hash to caller in real world; we will filter it out in controller
            return user;
        }

        public User? Login(LoginDTO dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null) return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (verificationResult == PasswordVerificationResult.Success ||
                verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                // Optionally re-hash if SuccessRehashNeeded
                if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.Password = _passwordHasher.HashPassword(user, dto.Password);
                    _context.Users.Update(user);
                    _context.SaveChanges();
                }

                return user;
            }

            return null;
        }

        public List<User> GetAllUsers()
        {
            // For safety, avoid sending password hash to consumers. We'll return full objects here but the controller will map them.
            return _context.Users.AsNoTracking().ToList();
        }
    }
}
