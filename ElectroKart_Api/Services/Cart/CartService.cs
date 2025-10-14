using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Cart;

namespace ElectroKart_Api.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        // Add a product to the cart
        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
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

        // Update the quantity of a cart item
        public async Task<bool> UpdateCartItemQuantityAsync(int userId, int itemId, int newQuantity)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any(i => i.Id == itemId)) return false;

            return await _cartRepository.UpdateItemQuantityAsync(itemId, newQuantity);
        }

        // Remove an item from the cart
        public async Task<bool> RemoveFromCartAsync(int userId, int itemId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any(i => i.Id == itemId)) return false;

            return await _cartRepository.RemoveItemFromCartAsync(itemId);
        }

        // Get the cart for a specific user
        public async Task<Cart?> GetCartByUserAsync(int userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }
    }
}
