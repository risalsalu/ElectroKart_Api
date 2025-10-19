using ElectroKart_Api.DTOs.Wishlist;
using ElectroKart_Api.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Wishlist
{
    public interface IWishlistService
    {
        Task<ApiResponse<bool>> AddProductToWishlistAsync(int userId, WishlistItemDto wishlistItemDto);
        Task<ApiResponse<List<WishlistItemDto>>> GetAllWishlistItemsAsync(int userId);
        Task<ApiResponse<bool>> DeleteWishlistItemAsync(int itemId);
    }
}
