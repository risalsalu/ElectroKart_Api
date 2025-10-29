using System;

namespace ElectroKart_Api.DTOs.Payments
{
    public class PaymentResponseDto
    {
        public string PaymentId { get; set; } = string.Empty;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string OrderReference { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
