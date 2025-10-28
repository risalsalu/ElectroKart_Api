using ElectroKart_Api.Data;
using ElectroKart_Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return await _context.Wishlists
                .Include(w => w.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<WishlistModel> CreateWishlistAsync(int userId)
        {
            var wishlist = new WishlistModel { UserId = userId };
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task AddItemAsync(int wishlistId, int productId)
        {
            var exists = await _context.WishlistItems
                .AnyAsync(i => i.WishlistId == wishlistId && i.ProductId == productId);

            if (exists) return;

            var wishlistItem = new WishlistItem
            {
                WishlistId = wishlistId,
                ProductId = productId
            };

            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ItemExistsAsync(int wishlistId, int productId)
        {
            return await _context.WishlistItems
                .AnyAsync(item => item.WishlistId == wishlistId && item.ProductId == productId);
        }

        public async Task<IEnumerable<WishlistItem>> GetAllWishlistItemsAsync(int userId)
        {
            return await _context.WishlistItems
                .Include(i => i.Product)
                .Include(i => i.Wishlist)
                .Where(i => i.Wishlist.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> DeleteWishlistItemByProductIdAsync(int userId, int productId)
        {
            var item = await _context.WishlistItems
                .Include(i => i.Wishlist)
                .FirstOrDefaultAsync(i => i.Wishlist.UserId == userId && i.ProductId == productId);

            if (item == null) return false;

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
