using ElectroKart_Api.Models;
using WishlistModel = ElectroKart_Api.Models.Wishlist;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories.Wishlist
{
    public interface IWishlistRepository
    {
        Task<WishlistModel?> GetWishlistByUserIdAsync(int userId);
        Task<WishlistModel> CreateWishlistAsync(int userId);
        Task AddItemAsync(int wishlistId, int productId);
        Task<bool> ItemExistsAsync(int wishlistId, int productId);
        Task<IEnumerable<WishlistItem>> GetAllWishlistItemsAsync(int userId);

        Task<bool> DeleteWishlistItemByProductIdAsync(int userId, int productId);
    }
}
