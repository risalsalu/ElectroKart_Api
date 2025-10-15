﻿using ElectroKart_Api.Models;

namespace ElectroKart_Api.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> AddUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);

        // --- ADD THIS ---
        Task<User?> GetUserByIdAsync(int id);
    }
}