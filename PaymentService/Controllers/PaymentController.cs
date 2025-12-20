using Microsoft.AspNetCore.Mvc;
using PaymentService.DTOs;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("make-payment")]
        public async Task<ActionResult<PaymentResponse>> MakePayment([FromBody] PaymentRequest request)
        {
            var result = await _paymentService.MakePaymentAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("status/{transactionId}")]
        public async Task<ActionResult> GetPaymentStatus(string transactionId)
        {
            var payment = await _paymentService.GetPaymentStatusAsync(transactionId);

            if (payment == null)
                return NotFound(new { message = "Payment not found" });

            return Ok(payment);
        }
    }
}
