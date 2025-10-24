using ElectroKart_Api.DTOs.Admin;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories;
using ElectroKart_Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly AppDbContext _context;

        public AdminUserService(IUserRepository userRepo, AppDbContext context)
        {
            _userRepo = userRepo;
            _context = context;
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

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();

            var totalRevenue = await _context.Orders
                .Where(o => o.Status == OrderStatus.Paid || o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount ?? 0);

            var pending = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            var shipped = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Shipped);
            var delivered = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Delivered);
            var cancelled = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled);

            return new DashboardStatsDto
            {
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                PendingOrders = pending,
                ShippedOrders = shipped,
                DeliveredOrders = delivered,
                CancelledOrders = cancelled
            };
        }
    }
}
