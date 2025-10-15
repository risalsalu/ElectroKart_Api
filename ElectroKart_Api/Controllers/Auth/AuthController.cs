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

            var loginResponse = await _authService.Login(dto);

            if (loginResponse == null)
            {
                return BadRequest(new { message = "Invalid Email or Password" });
            }

            return Ok(loginResponse);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            if (tokenRequestDto is null)
            {
                return BadRequest("Invalid client request");
            }

            var newTokens = await _authService.RefreshTokenAsync(tokenRequestDto);

            if (newTokens is null)
            {
                return Unauthorized("Invalid tokens");
            }

            return Ok(newTokens);
        }

        [HttpGet("GetAllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsers();
            var sanitized = users.Select(u => new { u.Id, u.Username, u.Email, u.CreatedAt }).ToList();
            return Ok(sanitized);
        }
    }
}