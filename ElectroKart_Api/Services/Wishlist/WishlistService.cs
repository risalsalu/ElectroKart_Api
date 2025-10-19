using ElectroKart_Api.DTOs.Wishlist;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Wishlist;

namespace ElectroKart_Api.Services.Wishlist
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        // Add a product to wishlist
        public async Task<ApiResponse<bool>> AddProductToWishlistAsync(int userId, WishlistItemDto wishlistItemDto)
        {
            try
            {
                // Get existing wishlist or create new one
                var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(userId)
                               ?? await _wishlistRepository.CreateWishlistAsync(userId);

                // Check if item already exists
                bool exists = await _wishlistRepository.ItemExistsAsync(wishlist.Id, wishlistItemDto.ProductId);
                if (exists)
                    return ApiResponse<bool>.FailureResponse("Product already in wishlist");

                // Add item
                await _wishlistRepository.AddItemAsync(wishlist.Id, wishlistItemDto.ProductId);

                return ApiResponse<bool>.SuccessResponse(true, "Product added to wishlist successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse("Failed to add product to wishlist", ex.Message);
            }
        }

        // Get all wishlist items for a user
        public async Task<ApiResponse<List<WishlistItemDto>>> GetAllWishlistItemsAsync(int userId)
        {
            try
            {
                var items = await _wishlistRepository.GetAllWishlistItemsAsync(userId);

                // Map to DTO including WishlistItem.Id
                var result = items.Select(i => new WishlistItemDto
                {
                    Id = i.Id,               // Include the WishlistItem ID
                    ProductId = i.ProductId
                }).ToList();

                return ApiResponse<List<WishlistItemDto>>.SuccessResponse(result, "Wishlist fetched successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WishlistItemDto>>.FailureResponse("Failed to fetch wishlist", ex.Message);
            }
        }

        // Delete a wishlist item by its ID
        public async Task<ApiResponse<bool>> DeleteWishlistItemAsync(int itemId)
        {
            try
            {
                bool deleted = await _wishlistRepository.DeleteWishlistItemAsync(itemId);
                if (!deleted)
                    return ApiResponse<bool>.FailureResponse("Wishlist item not found");

                return ApiResponse<bool>.SuccessResponse(true, "Wishlist item removed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse("Failed to delete wishlist item", ex.Message);
            }
        }
    }
}
