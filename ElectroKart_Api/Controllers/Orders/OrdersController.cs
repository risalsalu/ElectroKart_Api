using ElectroKart_Api.DTOs.Orders;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectroKart_Api.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId() => int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        private bool IsAdmin() => User.IsInRole("Admin");

        [HttpPost("checkout")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto dto)
        {
            var userId = GetUserId();
            var response = await _orderService.CreateOrderAsync(userId, dto);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var response = await _orderService.GetOrdersByUserIdAsync(userId);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var userId = GetUserId();
            var response = await _orderService.GetOrderByIdAsync(orderId, userId, IsAdmin());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPatch("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] string status)
        {
            var response = await _orderService.UpdateOrderStatusAsync(orderId, status);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("all-orders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var response = await _orderService.GetAllOrdersAsync();
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }
    }
}
