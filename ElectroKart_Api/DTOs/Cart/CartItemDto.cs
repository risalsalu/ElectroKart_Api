using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.DTOs.Cart
{
    public class CartItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}