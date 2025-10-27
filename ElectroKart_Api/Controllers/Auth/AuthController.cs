using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Auth;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ElectroKart_Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private CookieOptions CookieOptions => new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<User>.FailureResponse("Invalid data"));

            var user = await _authService.Register(dto);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role
            }, "Registered successfully"));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid data"));

            var loginResponse = await _authService.Login(dto);

            var accessCookieOptions = CookieOptions;
            accessCookieOptions.Expires = DateTime.UtcNow.AddMinutes(15);
            Response.Cookies.Append("AccessToken", loginResponse.AccessToken!, accessCookieOptions);

            var refreshCookieOptions = CookieOptions;
            refreshCookieOptions.Expires = DateTime.UtcNow.AddDays(7);
            Response.Cookies.Append("RefreshToken", loginResponse.RefreshToken!, refreshCookieOptions);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                loginResponse.AccessToken,
                loginResponse.RefreshToken,
                loginResponse.Username,
                loginResponse.Email,
                loginResponse.Role
            }, "Login successful"));
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRefreshDto? dto)
        {
            string? expiredToken = dto?.AccessToken;
            string? refreshToken = dto?.RefreshToken;

            if (string.IsNullOrEmpty(expiredToken))
            {
                expiredToken = Request.Cookies["AccessToken"];
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                refreshToken = Request.Cookies["RefreshToken"];
            }

            if (string.IsNullOrEmpty(expiredToken) || string.IsNullOrEmpty(refreshToken))
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid token data."));

            var newTokens = await _authService.RefreshToken(expiredToken, refreshToken);

            var accessCookieOptions = CookieOptions;
            accessCookieOptions.Expires = DateTime.UtcNow.AddMinutes(15);
            Response.Cookies.Append("AccessToken", newTokens.AccessToken!, accessCookieOptions);

            var refreshCookieOptions = CookieOptions;
            refreshCookieOptions.Expires = DateTime.UtcNow.AddDays(7);
            Response.Cookies.Append("RefreshToken", newTokens.RefreshToken!, refreshCookieOptions);

            return Ok(ApiResponse<object>.SuccessResponse(newTokens, "Token refreshed successfully"));
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var expiredCookie = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1),
                Path = "/"
            };

            Response.Cookies.Append("AccessToken", "", expiredCookie);
            Response.Cookies.Append("RefreshToken", "", expiredCookie);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Logged out successfully"));
        }
    }
}
