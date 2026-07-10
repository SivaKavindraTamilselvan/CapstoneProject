using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IOrderRepsository : IRepository<int, Order>
{
    public Task<Order?> GetOrderByOrderId(int orderId);
    public Task<List<Order>> GetOrdersForUser(int userId, OrderFilterParams filters);
    public Task<List<Order>> GetOrdersForAdmin(AdminOrderFilterParams filters,OrderFilterParams orderFilterParams);
    public Task<List<Order>> GetOrdersForVendor(int vendorId, OrderFilterParams filters);
    public Task<List<Order>> GetPendingOrdersByAddress(int address);
}