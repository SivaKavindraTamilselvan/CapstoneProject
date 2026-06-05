using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IPaymentService
{
    Task<ResponseRazorpayOrderDTO> CreateRazorpayOrder(int orderId);
    Task<string> VerifyRazorpayPayment(RequestVerifyRazorpayPaymentDTO request);
}