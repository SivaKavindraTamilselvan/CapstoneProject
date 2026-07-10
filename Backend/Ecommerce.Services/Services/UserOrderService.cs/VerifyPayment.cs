using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserOrderService : IUserOrderService
{
    public async Task OnPaymentVerified(int orderId)
    {
        var order = await _orderRepsository.Get(orderId);
        if(order == null)
        {
            throw new DataNotFoundException("Order not found");
        }
        await _orderService.ConfirmOrderStatus(orderId,true);
        await _userCartService.DeleteAllCart(order.UserId);
        var shipment = await _shipmentRepsository.GetShipmentByOrderId(orderId);
        foreach(var items in shipment)
        {
            items.ShipmentStatusId = 2;
            await _shipmentRepsository.Update(items.ShipmentId,items);
        }
    }
    public async Task OnPaymentFailed(int orderId)
    {
        var order = await _orderRepsository.Get(orderId);
        if(order == null)
        {
            throw new DataNotFoundException("Order not found");
        }
        await _orderService.ConfirmOrderStatus(orderId,false);
        var shipment = await _shipmentRepsository.GetShipmentByOrderId(orderId);
        foreach(var items in shipment)
        {
            items.ShipmentStatusId = 3;
            await _shipmentRepsository.Update(items.ShipmentId,items);
        }
    }
}