using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Wishlist;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Wishlist
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        // 🔹 Add product to wishlist
        public async Task AddProductToWishlistAsync(int userId, int productId)
        {
            // 1️⃣ Get user's wishlist
            var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(userId);

            // 2️⃣ If user doesn’t have one, create it
            if (wishlist == null)
            {
                wishlist = await _wishlistRepository.CreateWishlistAsync(userId);
            }

            // 3️⃣ Prevent duplicates
            bool itemExists = await _wishlistRepository.ItemExistsAsync(wishlist.Id, productId);
            if (itemExists)
            {
                return; // Product already exists, skip adding again
            }

            // 4️⃣ Add item to wishlist
            await _wishlistRepository.AddItemAsync(wishlist.Id, productId);
        }

        // 🔹 Get all wishlist items for a user
        public async Task<IEnumerable<WishlistItem>> GetAllWishlistItemsAsync(int userId)
        {
            return await _wishlistRepository.GetAllWishlistItemsAsync(userId);
        }

        // 🔹 Delete wishlist item by ID
        public async Task<bool> DeleteWishlistItemAsync(int itemId)
        {
            return await _wishlistRepository.DeleteWishlistItemAsync(itemId);
        }
    }
}
