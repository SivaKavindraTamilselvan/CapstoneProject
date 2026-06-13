using Ecommerce.DTOs.Shipment;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IShipmentItemsRepsository : IRepository<int, ShipmentItems>
{
    public Task<List<ShipmentItems>> GetPendingPackedTheShipmentItemsByShipmentId(int shipmentId);
    public Task<List<OrderItems>> GetOrderItemsByShippingId(int shipmentId);
    public Task<List<Shipment>> GetAllNotDeliveredOrderByOrderId(int orderId);

}