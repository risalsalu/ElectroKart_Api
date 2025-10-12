using ElectroKart_Api.DTOs.Cart;
using ElectroKart_Api.Services.CartServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElectroKart_Api.Controllers.Cart
{
    [ApiController]
    [Route("api/[controller]")]
    // --- CHANGE THIS ATTRIBUTE ---
    [Authorize(Roles = "User")] // Or "Customer", whatever you call your non-admin users
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto cartItemDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim not found or invalid.");
            }

            await _cartService.AddToCartAsync(userId, cartItemDto.ProductId, cartItemDto.Quantity);
            return Ok(new { message = "Product added to cart successfully." });
        }
    }
}