﻿using ElectroKart_Api.DTOs.Wishlist;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Services.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;

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

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistItemDto wishlistItemDto)
        {
            if (wishlistItemDto == null)
                return BadRequest(ApiResponse<WishlistItemDto>.FailureResponse("Invalid request body"));

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return Unauthorized(ApiResponse<WishlistItemDto>.FailureResponse("User ID claim not found or invalid"));

            var result = await _wishlistService.AddProductToWishlistAsync(userId, wishlistItemDto);

            if (!result.Success)
                return BadRequest(result);

            var responseData = new WishlistItemDto
            {
                Id = result.Data ? 0 : 0,
                ProductId = wishlistItemDto.ProductId
            };

            return Ok(ApiResponse<WishlistItemDto>.SuccessResponse(responseData, "Product added to wishlist successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWishlistItems()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return Unauthorized(ApiResponse<List<WishlistItemDto>>.FailureResponse("User ID claim not found or invalid"));

            var result = await _wishlistService.GetAllWishlistItemsAsync(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteWishlistItem(int itemId)
        {
            if (itemId <= 0)
                return BadRequest(ApiResponse<bool>.FailureResponse("Invalid item ID"));

            var result = await _wishlistService.DeleteWishlistItemAsync(itemId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
