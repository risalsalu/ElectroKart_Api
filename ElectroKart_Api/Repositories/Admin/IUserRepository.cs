using ElectroKart_Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories
{
    public interface IUserRepository
    {
        Task<int> GetCountAsync();
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(User user);
    }
}
