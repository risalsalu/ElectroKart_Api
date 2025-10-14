using ElectroKart_Api.DTOs.Orders;
using ElectroKart_Api.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace ElectroKart_Api.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/Orders/{userId}
        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateOrder(int userId, [FromBody] OrderDto orderDto)
        {
            if (orderDto == null || orderDto.Items == null || !orderDto.Items.Any())
                return BadRequest("Order must have at least one item.");

            var createdOrder = await _orderService.CreateOrderAsync(userId, orderDto);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrder.Id }, createdOrder);
        }

        // GET: api/Orders/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound("No orders found for this user.");

            return Ok(orders);
        }

        // GET: api/Orders/{orderId}
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound("Order not found.");

            return Ok(order);
        }
    }
}
