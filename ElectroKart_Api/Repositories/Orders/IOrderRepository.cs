﻿using ElectroKart_Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId); // int now
        Task UpdateOrderStatusAsync(Order order, OrderStatus status);
    }
}
