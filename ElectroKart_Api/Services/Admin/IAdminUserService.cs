using ElectroKart_Api.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Admin
{
    public interface IAdminUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> ToggleBlockUserAsync(int userId);
    }
}
