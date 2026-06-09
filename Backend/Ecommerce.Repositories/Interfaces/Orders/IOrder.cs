using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IOrderRepsository : IRepository<int,Order>
{
    public Task<List<Order>> GetPendingOrdersByAddress(int address);
    public Task<List<Order>> GetOrdersByUserId(int? status,int pageNumber,int pageSize,int userId);
}