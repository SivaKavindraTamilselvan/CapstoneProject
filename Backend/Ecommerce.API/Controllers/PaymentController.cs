using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment([FromBody] RequestCreatePaymentDTO request)
    {
        var result = await _paymentService.CreatePayment(
            request.OrderId,
            request.ModeOfPaymentId
        );

        return Ok(result);
    }

    [HttpPost("verify-razorpay-payment")]
    public async Task<IActionResult> VerifyRazorpayPayment([FromBody] RequestVerifyRazorpayPaymentDTO request)
    {
        var result = await _paymentService.VerifyRazorpayPayment(request);
        return Ok(new { message = result });
    }

    [HttpPost("payment-failed")]
    public async Task<IActionResult> PaymentFailed([FromBody] RequestPaymentFailedDTO request)
    {
        var result = await _paymentService.StorePaymentFailure(request);
        return Ok(new { message = result });
    }
}