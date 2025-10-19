using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.DTOs.Orders
{
    public class CreateOrderRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "COD";

        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
