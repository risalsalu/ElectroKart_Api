using System;
using System.Collections.Generic;

namespace ElectroKart_Api.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending"; // <-- Added

        public DateTime CreatedAt { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
