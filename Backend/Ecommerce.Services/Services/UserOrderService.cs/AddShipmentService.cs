using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserOrderService : IUserOrderService
{
    private async Task<List<SelectedCartInventory>> GetTheInventoryPickupAddress(List<CartItems> cartItems, Address address, int cod)
    {
        var selectedItems = new List<SelectedCartInventory>();
        foreach (var list in cartItems)
        {
            var inventories = list.ProductVariant!.Inventories.Where(i => i.AvailableQuantity >= list.Qunatity).ToList();
            Inventory? bestInventory = null;
            decimal bestRate = decimal.MaxValue;
            int bestEdd = 0;

            foreach (var inventory in inventories)
            {
                var request = new ServiceabilityRequestDTO
                {
                    PickupPostcode = inventory.Address!.PinCode,
                    DeliveryPostcode = address.PinCode,
                    Weight = list.Qunatity * list.ProductVariant.WeightInKgs,
                    Cod = cod
                };
                var courier = await _shipRocketService.CheckServiceability(request);

                if (courier != null && courier.Rate < bestRate)
                {
                    bestRate = courier.Rate;
                    bestInventory = inventory;
                    bestEdd = int.Parse(courier.EstimatedDeliveryDays);
                }
            }
            if (bestInventory == null)
            {
                throw new DataNotFoundException("No courier available for product");
            }
            selectedItems.Add(new SelectedCartInventory
            {
                CartItem = list,
                Inventory = bestInventory,
                ShippingRate = bestRate,
                EstimatedDeliveryDays = bestEdd,
                ExpectedDeliveryDate = DateTime.Now.AddDays(2 + bestEdd)
            });
        }
        return selectedItems;
    }
    private async Task<Shipment> CreateShipment(Order order, Address pickupAddress, decimal shippingCharge, DateTime dateTime)
    {
        RequestAddShipmentDTO requestAddShipmentDTO = new RequestAddShipmentDTO();
        requestAddShipmentDTO.OrderId = order.OrderId;
        requestAddShipmentDTO.PickupAddressId = pickupAddress.AddressId;
        requestAddShipmentDTO.ExpectedDeliveryDate = dateTime;
        var createdShipment = await _shipmentService.CreateShipment(requestAddShipmentDTO);
        return createdShipment;
    }
    private async Task CreateShipmentTracking(Shipment shipment)
    {
        RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO = new RequestAddShipmentTrackingDTO();
        requestAddShipmentTrackingDTO.ShipmentId = shipment.ShipmentId;
        requestAddShipmentTrackingDTO.ShipmentStatusId = shipment.ShipmentStatusId;
        requestAddShipmentTrackingDTO.Location = "Warehouse";
        requestAddShipmentTrackingDTO.Remarks = "Shipment Created Successfully. Items is Warehouse";
        await _shipmentService.CreateShipmentTracking(requestAddShipmentTrackingDTO);
    }
}