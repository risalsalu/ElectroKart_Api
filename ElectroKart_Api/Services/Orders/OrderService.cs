using ElectroKart_Api.DTOs.Orders;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Orders;
using ElectroKart_Api.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly AppDbContext _context;

        public OrderService(IOrderRepository orderRepository, AppDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(int userId, CreateOrderRequestDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("Order must have at least one item.");

            var items = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var dtoItem in dto.Items)
            {
                var product = await _context.Products.FindAsync(dtoItem.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {dtoItem.ProductId} not found.");

                items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = dtoItem.Quantity,
                    UnitPrice = product.Price
                });

                totalAmount += product.Price * dtoItem.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Items = items
            };

            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            return MapOrderToDto(createdOrder);
        }

        public async Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(string orderId, int userId)
        {
            if (!orderId.StartsWith("order_") || !int.TryParse(orderId.Replace("order_", ""), out int id))
                return null;

            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return null;

            return MapOrderToDto(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, int userId, string status)
        {
            if (!orderId.StartsWith("order_") || !int.TryParse(orderId.Replace("order_", ""), out int id))
                return false;

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                return false;

            if (userId != 0 && order.UserId != userId)
                return false;

            if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
                return false;

            await _orderRepository.UpdateOrderStatusAsync(order, newStatus);
            return true;
        }

        private OrderResponseDto MapOrderToDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };
        }
    }
}
