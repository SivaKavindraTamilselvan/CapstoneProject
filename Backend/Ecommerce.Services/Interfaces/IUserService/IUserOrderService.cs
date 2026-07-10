using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IUserOrderService
{
    public Task OnPaymentFailed(int orderId);
    public Task OnPaymentVerified(int orderId);
    public Task<ResponseAddOrderDTO> AddOrder(RequestAddOrderDTO requestAddOrderDTO, int userId);
}