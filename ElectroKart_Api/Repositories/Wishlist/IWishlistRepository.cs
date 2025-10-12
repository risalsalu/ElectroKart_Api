using ElectroKart_Api.Models;
using WishlistModel = ElectroKart_Api.Models.Wishlist;

namespace ElectroKart_Api.Repositories.Wishlist
{
    public interface IWishlistRepository
    {
        Task<WishlistModel?> GetWishlistByUserIdAsync(int userId);

        Task<WishlistModel> CreateWishlistAsync(int userId);

        Task AddItemAsync(int wishlistId, int productId);

        Task<bool> ItemExistsAsync(int wishlistId, int productId);
    }
}