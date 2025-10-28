using ElectroKart_Api.DTOs.Wishlist;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Repositories.Wishlist;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Wishlist
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<ApiResponse<bool>> AddProductToWishlistAsync(int userId, WishlistItemDto wishlistItemDto)
        {
            var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(userId)
                           ?? await _wishlistRepository.CreateWishlistAsync(userId);

            bool exists = await _wishlistRepository.ItemExistsAsync(wishlist.Id, wishlistItemDto.ProductId);
            if (exists)
                return ApiResponse<bool>.FailureResponse("Product already in wishlist");

            await _wishlistRepository.AddItemAsync(wishlist.Id, wishlistItemDto.ProductId);

            return ApiResponse<bool>.SuccessResponse(true, "Product added to wishlist successfully");
        }

        public async Task<ApiResponse<List<WishlistItemDto>>> GetAllWishlistItemsAsync(int userId)
        {
            var items = await _wishlistRepository.GetAllWishlistItemsAsync(userId);

            if (items == null || !items.Any())
                return ApiResponse<List<WishlistItemDto>>.SuccessResponse(new List<WishlistItemDto>(), "Wishlist is empty");

            var mappedItems = items
                .Where(i => i != null && i.Product != null)
                .Select(i => new WishlistItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    ImageUrl = i.Product?.ImageUrl ?? string.Empty,
                    Price = i.Product?.Price ?? 0
                })
                .ToList();

            return ApiResponse<List<WishlistItemDto>>.SuccessResponse(mappedItems, "Wishlist items fetched successfully");
        }

        public async Task<ApiResponse<bool>> DeleteWishlistItemAsync(int userId, int productId)
        {
            var deleted = await _wishlistRepository.DeleteWishlistItemByProductIdAsync(userId, productId);

            if (!deleted)
                return ApiResponse<bool>.FailureResponse("Wishlist item not found");

            return ApiResponse<bool>.SuccessResponse(true, "Product removed from wishlist successfully");
        }
    }
}
