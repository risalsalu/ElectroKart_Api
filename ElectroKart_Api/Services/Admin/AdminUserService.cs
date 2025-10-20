using ElectroKart_Api.DTOs.Admin;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserRepository _userRepo;

        public AdminUserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllUsersAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                IsBlocked = u.IsBlocked
            }).ToList();
        }

        public async Task<bool> ToggleBlockUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsBlocked = !user.IsBlocked;
            await _userRepo.UpdateAsync(user);
            return true;
        }
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;
            await _userRepo.DeleteAsync(user);
            return true;
        }
    }
}
