using ElectroKart_Api.DTOs.Orders;
using ElectroKart_Api.Helpers;
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

        public async Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderRequestDto dto)
        {
            try
            {
                if (dto.Items == null || !dto.Items.Any())
                    return ApiResponse<OrderResponseDto>.FailureResponse("Order must have at least one item");

                var items = new List<OrderItem>();
                decimal totalAmount = 0;

                foreach (var dtoItem in dto.Items)
                {
                    var product = await _context.Products.FindAsync(dtoItem.ProductId);
                    if (product == null)
                        return ApiResponse<OrderResponseDto>.FailureResponse($"Product ID {dtoItem.ProductId} not found");

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
                    TotalAmount = totalAmount, // decimal assigned safely
                    Items = items
                };

                var created = await _orderRepository.CreateOrderAsync(order);

                return ApiResponse<OrderResponseDto>.SuccessResponse(MapOrderToDto(created), "Order created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderResponseDto>.FailureResponse("Failed to create order", ex.Message);
            }
        }

        public async Task<ApiResponse<List<OrderResponseDto>>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var dtos = orders.Select(MapOrderToDto).ToList();
            return ApiResponse<List<OrderResponseDto>>.SuccessResponse(dtos, "Orders fetched successfully");
        }

        public async Task<ApiResponse<OrderResponseDto>> GetOrderByIdAsync(string orderId, int userId, bool isAdmin = false)
        {
            if (!TryParseOrderId(orderId, out int id))
                return ApiResponse<OrderResponseDto>.FailureResponse("Invalid order ID");

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return ApiResponse<OrderResponseDto>.FailureResponse("Order not found");

            if (!isAdmin && order.UserId != userId)
                return ApiResponse<OrderResponseDto>.FailureResponse("Unauthorized access");

            return ApiResponse<OrderResponseDto>.SuccessResponse(MapOrderToDto(order), "Order fetched successfully");
        }

        public async Task<ApiResponse<bool>> UpdateOrderStatusAsync(string orderId, string status)
        {
            if (!TryParseOrderId(orderId, out int id))
                return ApiResponse<bool>.FailureResponse("Invalid order ID");

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return ApiResponse<bool>.FailureResponse("Order not found");

            if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
                return ApiResponse<bool>.FailureResponse("Invalid status value");

            await _orderRepository.UpdateOrderStatusAsync(order, newStatus);
            return ApiResponse<bool>.SuccessResponse(true, "Order status updated successfully");
        }

        public async Task<ApiResponse<List<OrderResponseDto>>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            var dtos = orders.Select(MapOrderToDto).ToList();
            return ApiResponse<List<OrderResponseDto>>.SuccessResponse(dtos, "All orders fetched successfully");
        }

        private OrderResponseDto MapOrderToDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = $"order_{order.Id}",
                UserId = order.UserId,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount.GetValueOrDefault(), 
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }

        private bool TryParseOrderId(string orderId, out int id)
        {
            id = 0;
            if (orderId.StartsWith("order_"))
                return int.TryParse(orderId.Replace("order_", ""), out id);
            return int.TryParse(orderId, out id);
        }
    }
}
