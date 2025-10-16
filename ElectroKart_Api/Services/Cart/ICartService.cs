using ElectroKart_Api.Models;

namespace ElectroKart_Api.Services.CartServices
{
    public interface ICartService
    {
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task<bool> UpdateCartItemQuantityAsync(int userId, int itemId, int newQuantity);
        Task<bool> RemoveFromCartAsync(int userId, int itemId);
        Task<Cart?> GetCartByUserAsync(int userId);
        Task<bool> ClearCartAsync(int userId);
    }
}
