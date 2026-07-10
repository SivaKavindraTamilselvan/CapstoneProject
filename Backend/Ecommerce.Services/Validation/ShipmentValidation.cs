using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ShipmentValidation : IShipmentValidation
{
    private readonly IShipmentItemsRepsository _shipmentItemsRepsository;
    private readonly IShipmentRepsository _shipmentRepsository;
    public ShipmentValidation(IShipmentItemsRepsository shipmentItemsRepsository, IShipmentRepsository shipmentRepsository)
    {
        _shipmentItemsRepsository = shipmentItemsRepsository;
        _shipmentRepsository = shipmentRepsository;
    }
    public async Task<Shipment> ValidateGetShipmentByOrderItemId(int orderItemId)
    {
        var shipment = await _shipmentRepsository.GetShipmentByOrderItemId(orderItemId);
        if (shipment == null)
        {
            throw new DataNotFoundException("Shipment Not Found");
        }
        return shipment;
    }
    public async Task<List<ShipmentItems>> ValidateGetPendingPackedTheShipmentItemsByShipmentId(int shipmentId)
    {
        var shipmentItems = await _shipmentItemsRepsository.GetPendingPackedTheShipmentItemsByShipmentId(shipmentId);
        if(shipmentItems.Count != 0)
        {
            throw new DataAlreadyRegisteredException("Still Every ShipmentItems Are Not Packed");
        }
        return shipmentItems;
    }
}