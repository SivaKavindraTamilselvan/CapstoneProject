using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;
public interface IShipmentValidation
{
    public Task<Shipment> ValidateGetShipmentByOrderItemId(int orderItemId);
    public Task<List<ShipmentItems>> ValidateGetPendingPackedTheShipmentItemsByShipmentId(int shipmentId);
}