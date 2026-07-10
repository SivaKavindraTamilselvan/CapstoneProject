using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;
public interface IPaymentService
{
    public Task<string> ProcessRazorpayRefund(int refundId, decimal refundAmount);
    Task<ResponseCreatePaymentDTO> CreatePayment(int orderId, int modeOfPaymentId);
    Task<string> VerifyRazorpayPayment(RequestVerifyRazorpayPaymentDTO request);
    Task<string> StorePaymentFailure(RequestPaymentFailedDTO request);
}