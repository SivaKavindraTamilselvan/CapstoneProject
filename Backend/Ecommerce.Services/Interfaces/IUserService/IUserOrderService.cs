using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IUserOrderService
{
    public Task<ResponseAddOrderDTO> AddOrder(RequestAddOrderDTO requestAddOrderDTO, int userId);
}