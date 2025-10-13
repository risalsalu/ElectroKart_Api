using ElectroKart_Api.DTOs.Payment;
using ElectroKart_Api.Services.Payment;
using Microsoft.AspNetCore.Mvc;

namespace ElectroKart_Api.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] PaymentRequestDto request)
        {
            var result = await _service.CreateOrderAsync(request);
            return Ok(result);
        }

        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto request)
        {
            var verified = await _service.VerifyPaymentAsync(request);
            if (verified) return Ok(new { status = "success" });
            return BadRequest(new { status = "failed" });
        }
    }
}
