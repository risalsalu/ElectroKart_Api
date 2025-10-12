using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroKart_Api.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // FIX: Changed property name for consistency (was ShoppingCartId)
        public int CartId { get; set; }
        // FIX: Updated the ForeignKey attribute to match the new property name
        [ForeignKey("CartId")]
        // FIX: Renamed navigation property to be simpler (was ShoppingCart)
        public Cart? Cart { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int Quantity { get; set; }
    }
}