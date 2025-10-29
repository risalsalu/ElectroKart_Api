using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.DTOs.Payments
{
    public class CreatePaymentRequestDto
    {
        [Required]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required, MaxLength(10)]
        public string Currency { get; set; } = "INR";

        public string? Description { get; set; }
    }
}
