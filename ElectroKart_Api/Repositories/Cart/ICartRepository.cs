using ElectroKart_Api.Models;
using CartModel = ElectroKart_Api.Models.Cart;

namespace ElectroKart_Api.Repositories.Cart
{
    public interface ICartRepository
    {
        Task<CartModel?> GetCartByUserIdAsync(int userId);
        Task<CartModel> CreateCartAsync(int userId);
        Task AddItemToCartAsync(int cartId, int productId, int quantity);
        Task<bool> UpdateItemQuantityAsync(int itemId, int newQuantity);
        Task<bool> RemoveItemFromCartAsync(int itemId);
        Task<bool> ClearCartAsync(int cartId);
    }
}
