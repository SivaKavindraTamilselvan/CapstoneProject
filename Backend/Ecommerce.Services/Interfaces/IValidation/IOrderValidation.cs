using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;
public interface IOrderValidation
{
    public Task<OrderItems> ValidateOrderItem(int orderItemId);
    public Task<List<OrderItems>> ValidateGetOrderItemsByVendor(int vendorId);
}