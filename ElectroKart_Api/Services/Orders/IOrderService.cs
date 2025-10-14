using ElectroKart_Api.DTOs.Orders;

namespace ElectroKart_Api.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(int userId, OrderDto orderDto);

 
        Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId);

 
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
    }
}
