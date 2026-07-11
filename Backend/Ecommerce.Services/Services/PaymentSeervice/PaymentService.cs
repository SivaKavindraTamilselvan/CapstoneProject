using System.Security.Cryptography;
using System.Text;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;

public class PaymentService : IPaymentService
{
    private readonly IOrderRepsository _orderRepsository;
    private readonly IPaymentRepsository _paymentRepsository;
    private readonly IConfiguration _configuration;
    private readonly IOrderService _orderService;

    public PaymentService(IOrderService orderService, IOrderRepsository orderRepsository, IPaymentRepsository paymentRepsository, IConfiguration configuration)
    {
        _orderRepsository = orderRepsository;
        _paymentRepsository = paymentRepsository;
        _configuration = configuration;
        _orderService = orderService;
    }

    public async Task<ResponseCreatePaymentDTO> CreatePayment(int orderId, int modeOfPaymentId)
    {
        var order = await _orderRepsository.Get(orderId);

        if (order == null)
            throw new DataNotFoundException("Order Not Found");

        var payment = new Ecommerce.Models.Payment
        {
            OrderId = order.OrderId,
            ModeOfPaymentId = modeOfPaymentId,
            Amount = order.FinalAmount,
            PaymentStatusId = 1, // Pending
            PaymentDate = DateTime.Now,
            CreatedAt = DateTime.Now
        };

        // COD
        if (modeOfPaymentId == 1)
        {
            var createdPayment = await _paymentRepsository.Create(payment);
            if (createdPayment == null)
            {
                throw new DataNotFoundException("Payment Not Created");
            }

            return new ResponseCreatePaymentDTO
            {
                PaymentId = createdPayment.PaymentId,
                OrderId = order.OrderId,
                RequiresOnlinePayment = false,
                Message = "COD payment created"
            };
        }

        decimal paymentAmount = order.FinalAmount;
        if(paymentAmount>=25000)
        {
            paymentAmount = 25000;
        }
        // Razorpay
        string keyId = _configuration["Razorpay:KeyId"] ?? string.Empty;
        string keySecret = _configuration["Razorpay:KeySecret"] ?? string.Empty;
        int amountInPaise = (int)Math.Round(paymentAmount * 100);

        RazorpayClient client = new RazorpayClient(keyId, keySecret);

        Dictionary<string, object> options = new Dictionary<string, object>
        {
            { "amount", amountInPaise },
            { "currency", "INR" },
            { "receipt", order.OrderNumber },
            { "payment_capture", 1 }
        };

        Razorpay.Api.Order razorpayOrder = client.Order.Create(options);

        string razorpayOrderId = razorpayOrder["id"].ToString();

        payment.PaymentGatewayOrderId = razorpayOrderId;

        var savedPayment = await _paymentRepsository.Create(payment);
        if (savedPayment == null)
        {
            throw new DataNotFoundException("Payment Not Created");
        }

        return new ResponseCreatePaymentDTO
        {
            PaymentId = savedPayment.PaymentId,
            OrderId = order.OrderId,
            RequiresOnlinePayment = true,
            RazorpayOrderId = razorpayOrderId,
            Key = keyId,
            Amount = amountInPaise,
            Currency = "INR",
            Message = "Razorpay order created"
        };
    }

    public async Task<string> VerifyRazorpayPayment(RequestVerifyRazorpayPaymentDTO request)
    {
        bool isValid = VerifySignature(request);

        if (!isValid)
            throw new Exception("Invalid Razorpay signature");

        var payments = await _paymentRepsository.GetAll();

        var payment = payments.FirstOrDefault(p =>
            p.OrderId == request.OrderId &&
            p.PaymentGatewayOrderId == request.RazorpayOrderId);

        if (payment == null)
            throw new DataNotFoundException("Payment Not Found");

        payment.PaymentGatewayTransactionId = request.RazorpayPaymentId;
        payment.PaymentStatusId = 2; // Success
        payment.PaymentDate = DateTime.Now;
        payment.UpdatedAt = DateTime.Now;

        await _paymentRepsository.Update(payment.PaymentId, payment);

        return "Payment Verified Successfully";
    }

    public async Task<string> StorePaymentFailure(RequestPaymentFailedDTO request)
    {
        var payments = await _paymentRepsository.GetAll();

        var payment = payments.FirstOrDefault(p =>
            p.OrderId == request.OrderId &&
            p.PaymentGatewayOrderId == request.RazorpayOrderId);

        if (payment == null)
            throw new DataNotFoundException("Payment Not Found");

        payment.PaymentStatusId = 3; // Failed
        payment.FailureReason =
            $"{request.ErrorCode} - {request.ErrorDescription} | Source: {request.ErrorSource} | Step: {request.ErrorStep} | Reason: {request.ErrorReason} | Field: {request.ErrorField}";

        payment.UpdatedAt = DateTime.Now;

        await _paymentRepsository.Update(payment.PaymentId, payment);

        return "Payment failure stored successfully";
    }

    private bool VerifySignature(RequestVerifyRazorpayPaymentDTO request)
    {
        string keySecret = _configuration["Razorpay:KeySecret"] ?? string.Empty;

        string payload = request.RazorpayOrderId + "|" + request.RazorpayPaymentId;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(keySecret));

        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

        string generatedSignature = BitConverter
            .ToString(hash)
            .Replace("-", "")
            .ToLower();

        return generatedSignature == request.RazorpaySignature;
    }
    public async Task<string> ProcessRazorpayRefund(int refundId, decimal refundAmount)
    {
        var payments = await _paymentRepsository.GetAll();

        var payment = payments.FirstOrDefault(p =>
            p.RefundId == refundId &&
            p.PaymentStatusId == 2);

        if (payment == null)
            throw new DataNotFoundException("Successful payment not found for refund");

        if (string.IsNullOrEmpty(payment.PaymentGatewayTransactionId))
            throw new Exception("Razorpay payment id not found");

        string keyId = _configuration["Razorpay:KeyId"] ?? string.Empty;
        string keySecret = _configuration["Razorpay:KeySecret"] ?? string.Empty;

        RazorpayClient client = new RazorpayClient(keyId, keySecret);

        int amountInPaise = (int)Math.Round(refundAmount * 100);

        Razorpay.Api.Payment razorpayPayment =
            client.Payment.Fetch(payment.PaymentGatewayTransactionId);

        Dictionary<string, object> options = new Dictionary<string, object>
    {
        { "amount", amountInPaise }
    };

        Razorpay.Api.Refund razorpayRefund = razorpayPayment.Refund(options);

        return razorpayRefund["id"].ToString();
    }
}