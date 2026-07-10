using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IUserOrderService _userOrderService;

    public PaymentController(IPaymentService paymentService,IUserOrderService userOrderService)
    {
        _userOrderService = userOrderService;
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
        await _userOrderService.OnPaymentVerified(request.OrderId);
        return Ok(new { message = result });
    }

    [HttpPost("payment-failed")]
    public async Task<IActionResult> PaymentFailed([FromBody] RequestPaymentFailedDTO request)
    {
        var result = await _paymentService.StorePaymentFailure(request);
        await _userOrderService.OnPaymentFailed(request.OrderId);
        return Ok(new { message = result });
    }
}