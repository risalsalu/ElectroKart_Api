using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.DTOs.Payments
{
    public class PaymentConfirmationDto
    {
        [Required]
        public string PaymentId { get; set; } = string.Empty;
    }
}
