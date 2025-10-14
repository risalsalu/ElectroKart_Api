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

        // 🔹 Get Wishlist for a specific user (with items)
        public async Task<WishlistModel?> GetWishlistByUserIdAsync(int userId)
        {
            return await _context.Wishlists
                .Include(w => w.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        // 🔹 Create a new Wishlist for user
        public async Task<WishlistModel> CreateWishlistAsync(int userId)
        {
            var wishlist = new WishlistModel { UserId = userId };
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        // 🔹 Add product to Wishlist
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

        // 🔹 Check if item already exists
        public async Task<bool> ItemExistsAsync(int wishlistId, int productId)
        {
            return await _context.WishlistItems
                .AnyAsync(item => item.WishlistId == wishlistId && item.ProductId == productId);
        }

        // 🔹 Get all wishlist items for a user (including product info)
        public async Task<IEnumerable<WishlistItem>> GetAllWishlistItemsAsync(int userId)
        {
            return await _context.WishlistItems
                .Include(i => i.Product)
                .Include(i => i.Wishlist)
                .Where(i => i.Wishlist.UserId == userId)
                .ToListAsync();
        }

        // 🔹 Delete wishlist item by ID
        public async Task<bool> DeleteWishlistItemAsync(int itemId)
        {
            var item = await _context.WishlistItems.FindAsync(itemId);
            if (item == null)
                return false;

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
