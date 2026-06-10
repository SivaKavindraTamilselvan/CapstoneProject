using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class OrderService : IOrderService
{
    public async Task ConfirmOrderStatus(int orderId,bool status)
    {
        var order = await _orderRepsository.Get(orderId);
        if (order == null)
        {
            throw new DataNotFoundException("Order Not Found");
        }
        order.OrderStatusId = status ? 2 : 5;
        order.UpdatedAt = DateTime.Now;
        await _orderRepsository.Update(order.OrderId, order);
        var shipment = await _shipmentRepsository.GetShipmentByOrderId(order.OrderId);
        foreach (var item in shipment)
        {
            item.ShipmentStatusId = 2;
            await _shipmentRepsository.Update(item.ShipmentId, item);
            RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO = new RequestAddShipmentTrackingDTO();
            requestAddShipmentTrackingDTO.ShipmentId = item.ShipmentId;
            requestAddShipmentTrackingDTO.ShipmentStatusId = item.ShipmentStatusId;
            requestAddShipmentTrackingDTO.Location = "Warehouse";
            requestAddShipmentTrackingDTO.Remarks = "Shipment Created Successfully. Items is Warehouse";
            await _shipmentService.CreateShipmentTracking(requestAddShipmentTrackingDTO);
        }

    }
}