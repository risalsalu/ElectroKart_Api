namespace ElectroKart_Api.DTOs.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }          // CartItem ID
        public int ProductId { get; set; }   // Product ID
        public int Quantity { get; set; }    // Quantity in cart
        public int CartCount { get; set; }   // Total items in user's cart
    }
}
