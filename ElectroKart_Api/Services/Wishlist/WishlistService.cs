using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Wishlist;


namespace ElectroKart_Api.Services.Wishlist
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task AddProductToWishlistAsync(int userId, int productId)
        {
            var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(userId);

            if (wishlist == null)
            {
                wishlist = await _wishlistRepository.CreateWishlistAsync(userId);
            }
            bool itemExists = await _wishlistRepository.ItemExistsAsync(wishlist.Id, productId);
            if (itemExists)
            {
                return;
            }
            await _wishlistRepository.AddItemAsync(wishlist.Id, productId);
        }
        public async Task<IEnumerable<WishlistItem>> GetAllWishlistItemsAsync(int userId)
        {
            return await _wishlistRepository.GetAllWishlistItemsAsync(userId);
        }
        public async Task<bool> DeleteWishlistItemAsync(int itemId)
        {
            return await _wishlistRepository.DeleteWishlistItemAsync(itemId);
        }
    }
}
