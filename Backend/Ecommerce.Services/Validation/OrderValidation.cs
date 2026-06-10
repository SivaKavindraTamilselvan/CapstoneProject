using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class OrderValidation :IOrderValidation
{
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IOrderRepsository _orderRepsository;
    public OrderValidation(IOrderItemRepsository orderItemRepsository,IOrderRepsository orderRepsository)
    {
        _orderItemRepsository = orderItemRepsository;
        _orderRepsository = orderRepsository;
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
    public async Task<List<OrderItems>> ValidateGetOrderItemsByVendor(int vendorId,int? status)
    {
        var Orders = await _orderItemRepsository.GetOrderItemsByVendor(vendorId,status);
        if(Orders.Count == 0)
        {
            throw new DataNotFoundException("No Order Found For The Particular Vendor");
        }
        return Orders;
    }
    public async Task<Order> ValidateOrder(int orderId)
    {
        var order = await _orderRepsository.Get(orderId);
        if(order == null)
        {
            throw new DataNotFoundException("Order Is Not Found");
        }
        return order;
    }
}