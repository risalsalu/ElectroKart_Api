using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.DTOs.Payments
{
    public class PaymentConfirmationDto
    {
        [Required, MaxLength(100)]
        public string PaymentId { get; set; } = string.Empty;

        [Required]
        public string OrderId { get; set; } = string.Empty;
    }
}
