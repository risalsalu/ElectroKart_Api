using ElectroKart_Api.DTOs.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(int userId, CreateOrderRequestDto dto);
        Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderResponseDto?> GetOrderByIdAsync(string orderId, int userId);
        Task<bool> UpdateOrderStatusAsync(string orderId, int userId, string status);
    }
}
