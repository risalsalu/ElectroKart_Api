using ElectroKart_Api.Models;
// FIX: Create a nickname 'CartModel' for your actual Cart model to avoid name conflict.
using CartModel = ElectroKart_Api.Models.Cart;

namespace ElectroKart_Api.Repositories.Cart
{
    public interface ICartRepository
    {
        // FIX: Use the 'CartModel' nickname for the return type.
        Task<CartModel?> GetCartByUserIdAsync(int userId);

        // FIX: Use the 'CartModel' nickname here as well.
        Task<CartModel> CreateCartAsync(int userId);

        Task AddItemToCartAsync(int cartId, int productId, int quantity);

        Task UpdateItemQuantityAsync(int itemId, int newQuantity);
    }
}