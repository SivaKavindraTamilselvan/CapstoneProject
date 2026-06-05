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

    [HttpPost("create-razorpay-order/{orderId}")]
    public async Task<IActionResult> CreateRazorpayOrder(int orderId)
    {
        var result = await _paymentService.CreateRazorpayOrder(orderId);
        return Ok(result);
    }

    [HttpPost("verify-razorpay-payment")]
    public async Task<IActionResult> VerifyRazorpayPayment(
        RequestVerifyRazorpayPaymentDTO request)
    {
        var result = await _paymentService.VerifyRazorpayPayment(request);
        return Ok(new { message = result });
    }
}