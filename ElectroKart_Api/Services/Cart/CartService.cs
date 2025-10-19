using ElectroKart_Api.DTOs.Cart;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.CartServices
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
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(userId)
                           ?? await _cartRepository.CreateCartAsync(userId);

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
                if (existingItem != null)
                    return ApiResponse<CartItemDto>.FailureResponse("Product is already in cart");

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

                return ApiResponse<CartItemDto>.SuccessResponse(response, "Product added to cart successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CartItemDto>.FailureResponse("Failed to add product to cart", ex.Message);
            }
        }

        public async Task<ApiResponse<CartItemDto>> UpdateCartItemQuantityAsync(int userId, int itemId, CartItemRequestDto request)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.Items.Any(i => i.Id == itemId))
                    return ApiResponse<CartItemDto>.FailureResponse("Cart item not found");

                await _cartRepository.UpdateItemQuantityAsync(itemId, request.Quantity);

                var updatedItem = cart.Items.First(i => i.Id == itemId);
                var response = new CartItemDto
                {
                    Id = updatedItem.Id,
                    ProductId = updatedItem.ProductId,
                    Quantity = request.Quantity,
                    CartCount = cart.Items.Count
                };

                return ApiResponse<CartItemDto>.SuccessResponse(response, "Cart item updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CartItemDto>.FailureResponse("Failed to update cart item", ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> RemoveFromCartAsync(int userId, int itemId)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.Items.Any(i => i.Id == itemId))
                    return ApiResponse<bool>.FailureResponse("Cart item not found");

                var removed = await _cartRepository.RemoveItemFromCartAsync(itemId);
                if (!removed) return ApiResponse<bool>.FailureResponse("Failed to remove cart item");

                return ApiResponse<bool>.SuccessResponse(true, "Cart item removed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse("Failed to remove cart item", ex.Message);
            }
        }

        public async Task<ApiResponse<List<CartItemDto>>> GetCartByUserAsync(int userId)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.Items.Any())
                    return ApiResponse<List<CartItemDto>>.SuccessResponse(new List<CartItemDto>(), "Cart is empty");

                var items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    CartCount = cart.Items.Count
                }).ToList();

                return ApiResponse<List<CartItemDto>>.SuccessResponse(items, "Cart fetched successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CartItemDto>>.FailureResponse("Failed to fetch cart", ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> ClearCartAsync(int userId)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null) return ApiResponse<bool>.FailureResponse("Cart not found");

                var cleared = await _cartRepository.ClearCartAsync(cart.Id);
                if (!cleared) return ApiResponse<bool>.FailureResponse("Cart is already empty");

                return ApiResponse<bool>.SuccessResponse(true, "Cart cleared successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse("Failed to clear cart", ex.Message);
            }
        }
    }
}
