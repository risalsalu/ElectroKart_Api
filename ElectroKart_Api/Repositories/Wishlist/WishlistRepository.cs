using ElectroKart_Api.Data;
using ElectroKart_Api.Models;
using Microsoft.EntityFrameworkCore;
using WishlistModel = ElectroKart_Api.Models.Wishlist;

namespace ElectroKart_Api.Repositories.Wishlist
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WishlistModel?> GetWishlistByUserIdAsync(int userId)
        {
            // Use .Include() to also load all the related WishlistItems
            return await _context.Wishlists
                .Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<WishlistModel> CreateWishlistAsync(int userId)
        {
            var wishlist = new WishlistModel { UserId = userId };
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task AddItemAsync(int wishlistId, int productId)
        {
            var wishlistItem = new WishlistItem
            {
                WishlistId = wishlistId,
                ProductId = productId
            };
            await _context.WishlistItems.AddAsync(wishlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ItemExistsAsync(int wishlistId, int productId)
        {
            // .AnyAsync() is an efficient way to check if at least one item
            // matching the condition exists, without loading the actual item.
            return await _context.WishlistItems
                .AnyAsync(item => item.WishlistId == wishlistId && item.ProductId == productId);
        }
    }
}