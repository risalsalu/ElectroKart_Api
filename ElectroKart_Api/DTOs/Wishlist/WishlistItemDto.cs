using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.DTOs.Wishlist
{
    public class WishlistItemDto

    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
