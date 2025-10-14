using ElectroKart_Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Wishlist
{
    public interface IWishlistService
    {
        Task AddProductToWishlistAsync(int userId, int productId);

        Task<IEnumerable<WishlistItem>> GetAllWishlistItemsAsync(int userId);

        Task<bool> DeleteWishlistItemAsync(int itemId);
    }
}
