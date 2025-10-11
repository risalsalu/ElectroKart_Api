using ElectroKart_Api.DTOs;
using ElectroKart_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElectroKart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Register")]
        public IActionResult Register(RegisterDTO Dto)
        {
            var user = _userService.Register(Dto);
            return Ok(new { message = " Register Successfully", user });
        }
        [HttpPost("Login")]
        public IActionResult Login(LoginDTO Dto)
        {
            var user = _userService.Login(Dto);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid Email or Password" });
            }
            return Ok(new { message = "Login Successfully", user });
        }
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }
    }
}
