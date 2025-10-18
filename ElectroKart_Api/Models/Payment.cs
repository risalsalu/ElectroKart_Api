using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroKart_Api.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string PaymentId { get; set; } = string.Empty;

        // FK to Orders.Id
        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        // External gateway reference
        [Required, MaxLength(100)]
        public string OrderReference { get; set; } = string.Empty;

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(10)]
        public string Currency { get; set; } = "INR";

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";

        // FK to Users.Id
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
