using ElectroKart_Api.DTOs;
using ElectroKart_Api.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ElectroKart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _userService;
        public AuthController(IAuthService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userService.Register(dto);
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

            var user = await _userService.Login(dto);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid Email or Password" });
            }

            var result = new { user.Id, user.Username, user.Email, user.CreatedAt };
            return Ok(new { message = "Login successfully", user = result });
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            var sanitized = users.Select(u => new { u.Id, u.Username, u.Email, u.CreatedAt }).ToList();
            return Ok(sanitized);
        }
    }
}