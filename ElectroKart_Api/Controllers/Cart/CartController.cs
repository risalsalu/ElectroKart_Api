using ElectroKart_Api.DTOs.Cart;
using ElectroKart_Api.Services.CartServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElectroKart_Api.Controllers.Cart
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return Unauthorized("User ID claim not found or invalid.");

            var cart = await _cartService.GetCartByUserAsync(userId);

            if (cart == null || cart.Items == null || !cart.Items.Any())
                return Ok(new { message = "Cart is empty." });

            // Break the circular reference
            foreach (var item in cart.Items)
            {
                item.Cart = null;
            }

            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto cartItemDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return Unauthorized("User ID claim not found or invalid.");

            await _cartService.AddToCartAsync(userId, cartItemDto.ProductId, cartItemDto.Quantity);
            return Ok(new { message = "Product added to cart successfully." });
        }

        [HttpPut("update/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(int itemId, [FromBody] CartItemDto cartItemDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return Unauthorized("User ID claim not found or invalid.");

            // ✅ Await the Task before using ! operator
            var updated = await _cartService.UpdateCartItemQuantityAsync(userId, itemId, cartItemDto.Quantity);
            if (!updated) return NotFound("Cart item not found.");

            return Ok(new { message = "Cart item updated successfully." });
        }

        [HttpDelete("remove/{itemId}")]
        public async Task<IActionResult> RemoveCartItem(int itemId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return Unauthorized("User ID claim not found or invalid.");

            // ✅ Await the Task before using ! operator
            var removed = await _cartService.RemoveFromCartAsync(userId, itemId);
            if (!removed) return NotFound("Cart item not found.");

            return Ok(new { message = "Cart item removed successfully." });
        }
    }
}
