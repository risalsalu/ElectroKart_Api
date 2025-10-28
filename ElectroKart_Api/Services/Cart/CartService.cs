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
        private readonly string _baseUrl = "http://localhost:7289"; 

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

            var dto = new CartItemDto
            {
                ItemId = addedItem.Id,
                ProductId = addedItem.ProductId,
                ProductName = addedItem.Product?.Name ?? "Unknown",
                ProductImage = FormatImageUrl(addedItem.Product?.ImageUrl),
                Price = addedItem.Product?.Price ?? 0,
                Quantity = addedItem.Quantity,
                Subtotal = (addedItem.Product?.Price ?? 0) * addedItem.Quantity
            };

            return ApiResponse<CartItemDto>.SuccessResponse(dto, "Product added to cart successfully.");
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

            var updatedItem = await _cartRepository.GetCartByUserIdAsync(userId);
            var updated = updatedItem.Items.First(i => i.Id == itemId);

            var dto = new CartItemDto
            {
                ItemId = updated.Id,
                ProductId = updated.ProductId,
                ProductName = updated.Product?.Name ?? "Unknown",
                ProductImage = FormatImageUrl(updated.Product?.ImageUrl),
                Price = updated.Product?.Price ?? 0,
                Quantity = updated.Quantity,
                Subtotal = (updated.Product?.Price ?? 0) * updated.Quantity
            };

            return ApiResponse<CartItemDto>.SuccessResponse(dto, "Cart item updated successfully.");
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
                ItemId = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "Unknown",
                ProductImage = FormatImageUrl(i.Product?.ImageUrl),
                Price = i.Product?.Price ?? 0,
                Quantity = i.Quantity,
                Subtotal = (i.Product?.Price ?? 0) * i.Quantity
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

        private string FormatImageUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            return imageUrl.StartsWith("http", System.StringComparison.OrdinalIgnoreCase)
                ? imageUrl
                : $"{_baseUrl}/{imageUrl.TrimStart('/')}";
        }
    }
}
