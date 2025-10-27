using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.DTOs.Wishlist
{
    public class WishlistItemDto
    {
        [Required]
        public int ProductId { get; set; } 
    }
}
