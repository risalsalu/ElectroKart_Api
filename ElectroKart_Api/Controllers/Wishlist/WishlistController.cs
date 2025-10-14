using ElectroKart_Api.DTOs.Wishlist;
using ElectroKart_Api.Services.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElectroKart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")] // Only users can access
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistItemDto wishlistItemDto)
        {
            if (wishlistItemDto == null)
                return BadRequest("Invalid request body.");

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return Unauthorized("User ID claim not found or invalid.");

            await _wishlistService.AddProductToWishlistAsync(userId, wishlistItemDto.ProductId);
            return Ok(new { message = "Product added to wishlist successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWishlistItems()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return Unauthorized("User ID claim not found or invalid.");

            var wishlistItems = await _wishlistService.GetAllWishlistItemsAsync(userId);
            return Ok(wishlistItems);
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteWishlistItem(int itemId)
        {
            if (itemId <= 0)
                return BadRequest("Invalid item ID.");

            var success = await _wishlistService.DeleteWishlistItemAsync(itemId);
            if (!success)
                return NotFound(new { message = "Wishlist item not found." });

            return Ok(new { message = "Item removed from wishlist successfully." });
        }
    }
}
