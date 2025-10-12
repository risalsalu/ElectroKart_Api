using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Cart;
using ElectroKart_Api.Services.CartServices;

namespace ElectroKart_Api.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            // Try to get the user's cart
            // NOTE: No changes needed here because 'var' infers the corrected 'Cart' type from the repository
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            // If no cart exists, create one
            if (cart == null)
            {
                cart = await _cartRepository.CreateCartAsync(userId);
            }

            // Check if the item already exists in the cart
            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                // Update existing quantity
                await _cartRepository.UpdateItemQuantityAsync(existingItem.Id, existingItem.Quantity + quantity);
            }
            else
            {
                // Add new item
                await _cartRepository.AddItemToCartAsync(cart.Id, productId, quantity);
            }
        }
    }
}