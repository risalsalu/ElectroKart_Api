using ElectroKart_Api.DTOs;
using ElectroKart_Api.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ElectroKart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IAuthService _userService;
        public HomeController(IAuthService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _userService.Register(dto);
            if (user == null)
            {
                return BadRequest(new { message = "Email already in use." });
            }

            // Return sanitized result (do not return password hash)
            var result = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt
            };

            return Ok(new { message = "Registered successfully", user = result });
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _userService.Login(dto);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid Email or Password" });
            }

            var result = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt
            };

            // In next step we will also return the JWT token here
            return Ok(new { message = "Login successfully", user = result });
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            var sanitized = users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.CreatedAt
            }).ToList();

            return Ok(sanitized);
        }
    }
}
