using ElectroKart_Api.DTOs.Cart;
using ElectroKart_Api.Exceptions;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Cart;
using ElectroKart_Api.Services.CartServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<ApiResponse<CartItemDto>> AddToCartAsync(int userId, CartItemRequestDto request)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId)
                       ?? await _cartRepository.CreateCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existingItem != null)
                throw new BadRequestException("Product is already in the cart.");

            await _cartRepository.AddItemToCartAsync(cart.Id, request.ProductId, request.Quantity);

            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            var addedItem = cart.Items.First(i => i.ProductId == request.ProductId);

            var response = new CartItemDto
            {
                Id = addedItem.Id,
                ProductId = addedItem.ProductId,
                Quantity = addedItem.Quantity,
                CartCount = cart.Items.Count
            };

            return ApiResponse<CartItemDto>.SuccessResponse(response, "Product added to cart successfully.");
        }

        public async Task<ApiResponse<CartItemDto>> UpdateCartItemQuantityAsync(int userId, int itemId, CartItemRequestDto request)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found for the user.");

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new NotFoundException("Cart item not found.");

            await _cartRepository.UpdateItemQuantityAsync(itemId, request.Quantity);

            var response = new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = request.Quantity,
                CartCount = cart.Items.Count
            };

            return ApiResponse<CartItemDto>.SuccessResponse(response, "Cart item updated successfully.");
        }

        public async Task<ApiResponse<bool>> RemoveFromCartAsync(int userId, int itemId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found for the user.");

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new NotFoundException("Cart item not found.");

            var removed = await _cartRepository.RemoveItemFromCartAsync(itemId);
            if (!removed)
                throw new BadRequestException("Failed to remove the cart item.");

            return ApiResponse<bool>.SuccessResponse(true, "Cart item removed successfully.");
        }

        public async Task<ApiResponse<List<CartItemDto>>> GetCartByUserAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found for the user.");

            var items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                CartCount = cart.Items.Count
            }).ToList();

            return ApiResponse<List<CartItemDto>>.SuccessResponse(items, "Cart fetched successfully.");
        }

        public async Task<ApiResponse<bool>> ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found for the user.");

            var cleared = await _cartRepository.ClearCartAsync(cart.Id);
            if (!cleared)
                throw new BadRequestException("Cart is already empty.");

            return ApiResponse<bool>.SuccessResponse(true, "Cart cleared successfully.");
        }
    }
}
