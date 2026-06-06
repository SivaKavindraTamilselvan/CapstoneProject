using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class OrderValidation :IOrderValidation
{
    private readonly IOrderItemRepsository _orderItemRepsository;
    public OrderValidation(IOrderItemRepsository orderItemRepsository)
    {
        _orderItemRepsository = orderItemRepsository;
    }
    public async Task<OrderItems> ValidateOrderItem(int orderItemId)
    {
        var order = await _orderItemRepsository.Get(orderItemId);
        if(order == null)
        {
            throw new DataNotFoundException("Order Item Is Not Found");
        }
        return order;
    }
    public async Task<List<OrderItems>> ValidateGetOrderItemsByVendor(int vendorId)
    {
        var Orders = await _orderItemRepsository.GetOrderItemsByVendor(vendorId);
        if(Orders.Count == 0)
        {
            throw new DataNotFoundException("No Order Found For The Particular Vendor");
        }
        return Orders;
    }
}