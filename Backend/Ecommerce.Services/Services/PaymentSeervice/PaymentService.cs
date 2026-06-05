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

    public PaymentService(
        IOrderRepsository orderRepsository,
        IPaymentRepsository paymentRepsository,
        IConfiguration configuration)
    {
        _orderRepsository = orderRepsository;
        _paymentRepsository = paymentRepsository;
        _configuration = configuration;
    }

    public async Task<ResponseRazorpayOrderDTO> CreateRazorpayOrder(int orderId)
    {
        var order = await _orderRepsository.Get(orderId);

        if (order == null)
        {
            throw new DataNotFoundException("Order Not Found");
        }

        string keyId = _configuration["Razorpay:KeyId"] ?? string.Empty;
        string keySecret = _configuration["Razorpay:KeySecret"] ?? string.Empty;

        RazorpayClient client = new RazorpayClient(keyId, keySecret);

        Dictionary<string, object> options = new Dictionary<string, object>();
        options.Add("amount", (int)(order.FinalAmount * 100));
        options.Add("currency", "INR");
        options.Add("receipt", order.OrderNumber);
        options.Add("payment_capture", 1);

        Razorpay.Api.Order razorpayOrder = client.Order.Create(options);

        string razorpayOrderId = razorpayOrder["id"].ToString();

        Ecommerce.Models.Payment payment = new Ecommerce.Models.Payment(); 
        payment.OrderId = order.OrderId;
        payment.ModeOfPaymentId = 2; // Razorpay / Online Payment
        payment.Amount = order.FinalAmount;
        payment.PaymentGatewayOrderId = razorpayOrderId;
        payment.PaymentStatusId = 2; // Pending
        payment.PaymentDate = DateTime.Now;
        payment.CreatedAt = DateTime.Now;

        await _paymentRepsository.Create(payment);

        return new ResponseRazorpayOrderDTO
        {
            OrderId = order.OrderId,
            RazorpayOrderId = razorpayOrderId,
            Key = keyId,
            Amount = order.FinalAmount,
            Currency = "INR"
        };
    }

    public async Task<string> VerifyRazorpayPayment(RequestVerifyRazorpayPaymentDTO request)
    {
        bool isValid = VerifySignature(request);

        if (!isValid)
        {
            throw new Exception("Invalid Razorpay signature");
        }

        var payments = await _paymentRepsository.GetAll();

        var payment = payments.FirstOrDefault(p =>
            p.OrderId == request.OrderId &&
            p.PaymentGatewayOrderId == request.RazorpayOrderId);

        if (payment == null)
        {
            throw new DataNotFoundException("Payment Not Found");
        }

        payment.PaymentGatewayTransactionId = request.RazorpayPaymentId;
        payment.PaymentStatusId = 2; // Success
        payment.UpdatedAt = DateTime.Now;

        await _paymentRepsository.Update(payment.PaymentId, payment);

        return "Payment Verified Successfully";
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
}