using Ecommerce.Data;
using Ecommerce.DTOs.Shipment;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ShipmentItemRepsository : AbstractRepository<int, ShipmentItems>, IShipmentItemsRepsository
{
    public ShipmentItemRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<List<ShipmentItems>> GetTheShipmentItemsByShipmentId(int shipmentId)
    {
        var shipment = await _ecommerceContext.ShipmentItems.Where(s => s.ShipmentId == shipmentId).ToListAsync();
        return shipment;
    }
    public async Task<List<ShipmentItems>> GetPendingPackedTheShipmentItemsByShipmentId(int shipmentId)
    {
        var shipment = await _ecommerceContext.ShipmentItems.Include(s => s.OrderItems).Where(s => s.ShipmentId == shipmentId && s.OrderItems!.OrderItemStatusId != 2).ToListAsync();
        return shipment;
    }
    public async Task<List<OrderItems>> GetOrderItemsByShippingId(int shipmentId)
    {
        var orderItem = await _ecommerceContext.ShipmentItems.Include(s=>s.Shipment).ThenInclude(o=>o.Order).Where(s => s.ShipmentId == shipmentId).Select(s => s.OrderItems!).ToListAsync();
        return orderItem;
    }
    public async Task<List<Shipment>> GetAllNotDeliveredOrderByOrderId(int orderId)
    {
        var shipment = await _ecommerceContext.Shipment.Where(s => s.OrderId == orderId && s.ShipmentStatusId != 8).ToListAsync();
        return shipment;
    }

}