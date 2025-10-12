using ElectroKart_Api.Data;
using ElectroKart_Api.Models;
using Microsoft.EntityFrameworkCore;
// FIX: Add the same alias here.
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

        // FIX: Use 'CartModel' as the return type.
        public async Task<CartModel?> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        // FIX: Use 'CartModel' for the return type and when creating a new object.
        public async Task<CartModel> CreateCartAsync(int userId)
        {
            var cart = new CartModel { UserId = userId }; // Use CartModel here
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task AddItemToCartAsync(int cartId, int productId, int quantity)
        {
            var cartItem = new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity
            };
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemQuantityAsync(int itemId, int newQuantity)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item != null)
            {
                item.Quantity = newQuantity;
                await _context.SaveChangesAsync();
            }
        }
    }
}