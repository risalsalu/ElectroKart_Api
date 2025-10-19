using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.DTOs.Payments
{
    public class CreatePaymentRequestDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required, MaxLength(10)]
        public string Currency { get; set; } = "INR";

        public string? Description { get; set; }
    }
}
