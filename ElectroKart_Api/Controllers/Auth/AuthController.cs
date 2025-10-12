using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Auth;
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

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _authService.Register(dto);
            if (user == null)
            {
                return BadRequest(new { message = "Email already in use." });
            }

            var result = new { user.Id, user.Username, user.Email, user.CreatedAt };
            return Ok(new { message = "Registered successfully", user = result });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (user, token) = await _authService.Login(dto);

            if (user == null || token == null)
            {
                return BadRequest(new { message = "Invalid Email or Password" });
            }

            return Ok(new
            {
                message = "Login successful",
                token,
                user = new { user.Id, user.Username, user.Email }
            });
        }

        [HttpGet("GetAllUsers")]
        [Authorize] // This endpoint is now protected
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsers();
            var sanitized = users.Select(u => new { u.Id, u.Username, u.Email, u.CreatedAt }).ToList();
            return Ok(sanitized);
        }
    }
}