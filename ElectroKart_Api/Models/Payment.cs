using System;
using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OrderId { get; set; } = string.Empty;

        public string? PaymentId { get; set; }

        public string? Signature { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = "Created";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Currency { get; internal set; } = string.Empty;
    }
}
