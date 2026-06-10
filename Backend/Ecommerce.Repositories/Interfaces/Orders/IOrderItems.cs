using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IOrderItemRepsository : IRepository<int, OrderItems>
{
    public Task<List<OrderItems>> GetPendingOrderByInventoryAddress(int addressId);
    public Task<List<OrderItems>> GetOrderByInventoryAddress(int? status, int vendorId, int pageNumber, int pageSize, int addressId);
    public Task<List<OrderItems>> GetOrderItemsByOrderId(int orderId);
    public Task<List<OrderItems>> GetCancelledOrderItemsByOrderId(int orderId);
    public Task<List<OrderItems>> GetOrderItemsByVendor(int vendorId);
}