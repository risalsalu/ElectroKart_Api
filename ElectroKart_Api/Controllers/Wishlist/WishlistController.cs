using ElectroKart_Api.DTOs.Wishlist;
using ElectroKart_Api.Services.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElectroKart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")] // Protects the entire controller
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
            // Get the user's ID from the token
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim not found or invalid.");
            }

            // Call the service to handle all the business logic
            await _wishlistService.AddProductToWishlistAsync(userId, wishlistItemDto.ProductId);

            return Ok(new { message = "Product added to wishlist successfully." });
        }
    }
}