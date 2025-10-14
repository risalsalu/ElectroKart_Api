using ElectroKart_Api.Models;
using CartModel = ElectroKart_Api.Models.Cart;

namespace ElectroKart_Api.Repositories.Cart
{
    public interface ICartRepository
    {
        // Get the cart for a user
        Task<CartModel?> GetCartByUserIdAsync(int userId);

        // Create a new cart for a user
        Task<CartModel> CreateCartAsync(int userId);

        // Add an item to a cart
        Task AddItemToCartAsync(int cartId, int productId, int quantity);

        // Update the quantity of a cart item
        Task<bool> UpdateItemQuantityAsync(int itemId, int newQuantity);

        // Remove an item from the cart
        Task<bool> RemoveItemFromCartAsync(int itemId);
    }
}
