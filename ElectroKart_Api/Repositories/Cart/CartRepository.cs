using ElectroKart_Api.Data;
using ElectroKart_Api.Models;
using Microsoft.EntityFrameworkCore;
using CartModel = ElectroKart_Api.Models.Cart;

namespace ElectroKart_Api.Repositories.Cart
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get cart for a user including items
        public async Task<CartModel?> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        // Create a new cart for a user
        public async Task<CartModel> CreateCartAsync(int userId)
        {
            var cart = new CartModel { UserId = userId };
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        // Add item to cart
        public async Task AddItemToCartAsync(int cartId, int productId, int quantity)
        {
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        // Update quantity of a cart item
        public async Task<bool> UpdateItemQuantityAsync(int itemId, int newQuantity)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item == null) return false;

            if (newQuantity <= 0)
            {
                _context.CartItems.Remove(item); // Remove item if quantity <= 0
            }
            else
            {
                item.Quantity = newQuantity;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Remove an item from the cart
        public async Task<bool> RemoveItemFromCartAsync(int itemId)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item == null) return false;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
