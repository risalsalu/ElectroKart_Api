using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Payments;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectroKart_Api.Controllers.Payments
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private int? GetUserId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : (int?)null;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] CreatePaymentRequestDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Request body cannot be null" });

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid request data" });

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<string> { Success = false, Message = "Unauthorized user" });

            var response = await _paymentService.InitiatePaymentAsync(dto, userId.Value);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentConfirmationDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Request body cannot be null" });

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid request data" });

            var response = await _paymentService.ConfirmPaymentAsync(dto);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpGet("by-order/{orderId:int}")]
        public async Task<IActionResult> GetPaymentByOrderId(int orderId)
        {
            if (orderId <= 0)
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid order ID" });

            var response = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            return StatusCode(response.Success ? 200 : 404, response);
        }
    }
}
