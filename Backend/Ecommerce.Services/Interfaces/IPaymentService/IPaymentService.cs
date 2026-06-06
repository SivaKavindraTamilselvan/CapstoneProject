using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;
public interface IPaymentService
{
    Task<ResponseCreatePaymentDTO> CreatePayment(int orderId, int modeOfPaymentId);
    Task<string> VerifyRazorpayPayment(RequestVerifyRazorpayPaymentDTO request);
    Task<string> StorePaymentFailure(RequestPaymentFailedDTO request);
}