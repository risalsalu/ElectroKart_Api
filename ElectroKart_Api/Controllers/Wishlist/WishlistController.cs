using ElectroKart_Api.DTOs.Wishlist;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Services.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectroKart_Api.Controllers
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

        private int GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                throw new UnauthorizedAccessException("User ID claim not found or invalid");
            return userId;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistItemDto wishlistItemDto)
        {
            if (wishlistItemDto == null)
                return BadRequest(ApiResponse<WishlistItemDto>.FailureResponse("Invalid request body"));

            try
            {
                var userId = GetUserId();
                var result = await _wishlistService.AddProductToWishlistAsync(userId, wishlistItemDto);

                if (!result.Success)
                    return BadRequest(ApiResponse<WishlistItemDto>.FailureResponse(result.Message));

                return Ok(ApiResponse<WishlistItemDto>.SuccessResponse(
                    new WishlistItemDto { ProductId = wishlistItemDto.ProductId },
                    "Product added to wishlist successfully"
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<WishlistItemDto>.FailureResponse(ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWishlistItems()
        {
            try
            {
                var userId = GetUserId();
                var result = await _wishlistService.GetAllWishlistItemsAsync(userId);

                if (!result.Success)
                    return BadRequest(ApiResponse<List<WishlistItemDto>>.FailureResponse(result.Message));

                return Ok(ApiResponse<List<WishlistItemDto>>.SuccessResponse(result.Data, "Wishlist items fetched successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<List<WishlistItemDto>>.FailureResponse(ex.Message));
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteWishlistItem(int productId)
        {
            if (productId <= 0)
                return BadRequest(ApiResponse<bool>.FailureResponse("Invalid product ID"));

            try
            {
                var userId = GetUserId();
                var result = await _wishlistService.DeleteWishlistItemAsync(userId, productId);

                if (!result.Success)
                    return NotFound(ApiResponse<bool>.FailureResponse(result.Message));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Product removed from wishlist successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<bool>.FailureResponse(ex.Message));
            }
        }
    }
}
