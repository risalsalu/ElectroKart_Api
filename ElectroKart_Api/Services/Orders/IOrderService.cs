using ElectroKart_Api.DTOs.Orders;
using ElectroKart_Api.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Orders
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderRequestDto dto);
        Task<ApiResponse<List<OrderResponseDto>>> GetOrdersByUserIdAsync(int userId);
        Task<ApiResponse<OrderResponseDto>> GetOrderByIdAsync(string orderId, int userId, bool isAdmin = false);
        Task<ApiResponse<bool>> UpdateOrderStatusAsync(string orderId, string status);
        Task<ApiResponse<List<OrderResponseDto>>> GetAllOrdersAsync(); // Admin only

        Task<ApiResponse<bool>> DeleteOrderAsync(string orderId);
    }
}
