using ElectroKart_Api.DTOs.Cart;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Services.CartServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        private int? GetUserIdFromClaims()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return null;

            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(ApiResponse<List<CartItemDto>>.FailureResponse("User ID claim not found or invalid"));

            var result = await _cartService.GetCartByUserAsync(userId.Value);

            if (result == null || result.Data == null || result.Data.Count == 0)
                return Ok(ApiResponse<List<CartItemDto>>.SuccessResponse(new List<CartItemDto>(), "Cart is empty"));

            var cartItems = result.Data.Select(item => new CartItemDto
            {
                ItemId = item.ItemId,
                ProductId = item.ProductId,
                ProductName = item.ProductName ?? "Unknown Product",
                ProductImage = item.ProductImage ?? string.Empty,
                Price = item.Price, // Must not be null
                Quantity = item.Quantity,
                Subtotal = item.Price * item.Quantity
            }).ToList();

            var totalAmount = cartItems.Sum(i => i.Subtotal);

            var response = new
            {
                Items = cartItems,
                TotalAmount = totalAmount
            };

            return Ok(ApiResponse<object>.SuccessResponse(response, "Cart fetched successfully"));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemRequestDto request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(ApiResponse<CartItemDto>.FailureResponse("User ID claim not found or invalid"));

            var result = await _cartService.AddToCartAsync(userId.Value, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("update/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(int itemId, [FromBody] CartItemRequestDto request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(ApiResponse<CartItemDto>.FailureResponse("User ID claim not found or invalid"));

            var result = await _cartService.UpdateCartItemQuantityAsync(userId.Value, itemId, request);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("remove/{itemId}")]
        public async Task<IActionResult> RemoveCartItem(int itemId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(ApiResponse<bool>.FailureResponse("User ID claim not found or invalid"));

            var result = await _cartService.RemoveFromCartAsync(userId.Value, itemId);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(ApiResponse<bool>.FailureResponse("User ID claim not found or invalid"));

            var result = await _cartService.ClearCartAsync(userId.Value);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
