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
            // Step 1: Get the user's wishlist.
            var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(userId);

            // Step 2: If the user doesn't have a wishlist, create one.
            if (wishlist == null)
            {
                wishlist = await _wishlistRepository.CreateWishlistAsync(userId);
            }

            // Step 3: Check if the product is already in the wishlist to prevent duplicates.
            bool itemExists = await _wishlistRepository.ItemExistsAsync(wishlist.Id, productId);
            if (itemExists)
            {
                // If it already exists, do nothing and exit the method.
                return;
            }

            // Step 4: If the item does not exist, add it.
            await _wishlistRepository.AddItemAsync(wishlist.Id, productId);
        }
    }
}