using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IUserOrderService
{
    public Task<decimal> GetWalletBalance(int userId);
    public Task<ShippingCheckResponseDTO> CheckService(RequestAddOrderDTO requestAddOrderDTO, int userId);
    public Task OnPaymentFailed(int orderId);
    public Task OnPaymentVerified(int orderId);
    public Task<ResponseAddOrderDTO> AddOrder(RequestAddOrderDTO requestAddOrderDTO, int userId, string idempotencyKey);
}