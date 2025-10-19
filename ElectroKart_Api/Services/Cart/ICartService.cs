using ElectroKart_Api.DTOs.Cart;
using ElectroKart_Api.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.CartServices
{
    public interface ICartService
    {
        Task<ApiResponse<CartItemDto>> AddToCartAsync(int userId, CartItemRequestDto request);
        Task<ApiResponse<CartItemDto>> UpdateCartItemQuantityAsync(int userId, int itemId, CartItemRequestDto request);
        Task<ApiResponse<bool>> RemoveFromCartAsync(int userId, int itemId);
        Task<ApiResponse<List<CartItemDto>>> GetCartByUserAsync(int userId);
        Task<ApiResponse<bool>> ClearCartAsync(int userId);
    }
}
