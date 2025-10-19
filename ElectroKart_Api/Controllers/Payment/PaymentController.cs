using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Payments;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectroKart_Api.Controllers
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

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] CreatePaymentRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid request data"
                });
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Unauthorized user"
                });
            }

            int userId = int.Parse(userIdClaim.Value);
            var response = await _paymentService.InitiatePaymentAsync(dto, userId);

            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentConfirmationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid request data"
                });
            }

            var response = await _paymentService.ConfirmPaymentAsync(dto);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpGet("by-order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(int orderId)
        {
            var response = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            return StatusCode(response.Success ? 200 : 404, response);
        }
    }
}
