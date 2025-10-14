using ElectroKart_Api.Models;

namespace ElectroKart_Api.Services.CartServices
{
    public interface ICartService
    {
        // Add a product to the user's cart
        Task AddToCartAsync(int userId, int productId, int quantity);

        // Update quantity of a cart item
        Task<bool> UpdateCartItemQuantityAsync(int userId, int itemId, int newQuantity);

        // Remove a cart item
        Task<bool> RemoveFromCartAsync(int userId, int itemId);

        // Get the cart for a user
        Task<Cart?> GetCartByUserAsync(int userId);
    }
}
