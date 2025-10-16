using ElectroKart_Api.Data;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Cart;
using Microsoft.EntityFrameworkCore;

namespace ElectroKart_Api.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly AppDbContext _context;

        public CartService(ICartRepository cartRepository, AppDbContext context)
        {
            _cartRepository = cartRepository;
            _context = context;
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            // ✅ Ensure product exists to prevent FK issues
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Product does not exist.");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId)
                       ?? await _cartRepository.CreateCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                await _cartRepository.UpdateItemQuantityAsync(existingItem.Id, existingItem.Quantity + quantity);
            }
            else
            {
                await _cartRepository.AddItemToCartAsync(cart.Id, productId, quantity);
            }
        }

        public async Task<bool> UpdateCartItemQuantityAsync(int userId, int itemId, int newQuantity)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any(i => i.Id == itemId)) return false;

            return await _cartRepository.UpdateItemQuantityAsync(itemId, newQuantity);
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int itemId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any(i => i.Id == itemId)) return false;

            return await _cartRepository.RemoveItemFromCartAsync(itemId);
        }

        public async Task<Cart?> GetCartByUserAsync(int userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            return await _cartRepository.ClearCartAsync(cart.Id);
        }
    }
}
