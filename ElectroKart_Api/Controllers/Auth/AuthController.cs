using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Auth;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroKart_Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Register endpoint
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<User>.FailureResponse("Invalid data"));

            var user = await _authService.Register(dto);
            if (user == null)
                return BadRequest(ApiResponse<User>.FailureResponse("Email already in use."));

            var result = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt
            };

            return Ok(ApiResponse<object>.SuccessResponse(result, "Registered successfully"));
        }

        // Login endpoint (return token only)
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid data"));

            var loginResponse = await _authService.Login(dto);
            if (loginResponse == null)
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid Email or Password"));

            // Return only the token string (plain text)
            return Content(loginResponse.AccessToken, "text/plain");
        }

        // Get all users (Admin only)
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsers();
            var sanitized = users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.CreatedAt
            }).ToList();

            return Ok(ApiResponse<object>.SuccessResponse(sanitized, "Users fetched successfully"));
        }
    }
}
