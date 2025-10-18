using ElectroKart_Api.DTOs.Payments;
using ElectroKart_Api.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectroKart_Api.Controllers.Payments
{
    [Route("api/[controller]")]
    [ApiController]
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
                return BadRequest(ModelState);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var payment = await _paymentService.InitiatePaymentAsync(dto, userId);
            return Ok(payment);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentConfirmationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _paymentService.ConfirmPaymentAsync(dto);

            if (!success)
                return BadRequest("Payment confirmation failed.");

            return Ok(new { message = "Payment confirmed successfully." });
        }
    }
}
