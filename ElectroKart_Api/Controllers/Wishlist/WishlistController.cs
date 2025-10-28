using ElectroKart_Api.DTOs.Wishlist;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Services.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectroKart_Api.Controllers.Wishlist
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int? TryGetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return null;
            return userId;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistItemDto wishlistItemDto)
        {
            if (wishlistItemDto == null)
                return BadRequest(ApiResponse<bool>.FailureResponse("Invalid request body"));

            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(ApiResponse<bool>.FailureResponse("Unauthorized"));

            var result = await _wishlistService.AddProductToWishlistAsync(userId.Value, wishlistItemDto);

            if (!result.Success) return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Product added to wishlist successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWishlistItems()
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(ApiResponse<List<WishlistItemDto>>.FailureResponse("Unauthorized"));

            var result = await _wishlistService.GetAllWishlistItemsAsync(userId.Value);

            if (!result.Success) return BadRequest(ApiResponse<List<WishlistItemDto>>.FailureResponse(result.Message));

            return Ok(ApiResponse<List<WishlistItemDto>>.SuccessResponse(result.Data, "Wishlist items fetched successfully"));
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteWishlistItem(int productId)
        {
            if (productId <= 0) return BadRequest(ApiResponse<bool>.FailureResponse("Invalid product ID"));

            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(ApiResponse<bool>.FailureResponse("Unauthorized"));

            var result = await _wishlistService.DeleteWishlistItemAsync(userId.Value, productId);

            if (!result.Success) return NotFound(ApiResponse<bool>.FailureResponse(result.Message));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Product removed from wishlist successfully"));
        }
    }
}
