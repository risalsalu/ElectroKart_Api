using System;
using System.Collections.Generic;

namespace ElectroKart_Api.DTOs.Orders
{
    public class OrderResponseDto
    {
        public string OrderId { get; set; } = string.Empty; 
        public int UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "COD";
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
