using ElectroKart_Api.Models;

namespace ElectroKart_Api.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);

        Task<List<Order>> GetOrdersByUserIdAsync(int userId);

        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}
