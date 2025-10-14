using ElectroKart_Api.DTOs.Orders;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Orders;

namespace ElectroKart_Api.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Create a new order
        public async Task<OrderDto> CreateOrderAsync(int userId, OrderDto orderDto)
        {
            // Map DTO to entity
            var order = new Order
            {
                UserId = userId,
                Status = orderDto.Status ?? "Pending",
                CreatedAt = DateTime.UtcNow,
                Items = orderDto.Items?.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList() ?? new List<OrderItem>()
            };

            // Calculate total amount
            order.TotalAmount = order.Items.Sum(i => i.Price * i.Quantity);

            // Save using repository
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // Map entity back to DTO
            return new OrderDto
            {
                Id = createdOrder.Id,
                Status = createdOrder.Status,
                TotalAmount = createdOrder.TotalAmount,
                CreatedAt = createdOrder.CreatedAt,
                Items = createdOrder.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
        }

        // Get all orders for a user
        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            }).ToList();
        }

        // Get a specific order by Id
        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
        }
    }
}
