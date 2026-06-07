using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class AdminReturnService : IAdminReturnService
{
    private readonly IReturnRepsository _returnRepsository;
    private readonly IShipRocketService _shipRocketService;
    private readonly IShipmentService _shipmentService;
    public AdminReturnService(IReturnRepsository returnRepsository, IShipRocketService shipRocketService, IShipmentService shipmentService)
    {
        _returnRepsository = returnRepsository;
        _shipRocketService = shipRocketService;
        _shipmentService = shipmentService;
    }
    public async Task<ResponseCreateReturnShipmentDTO> CreateReturnShipment(int returnId)
    {
        var user = await _returnRepsository.GetTheReturnUserByReturnId(returnId);
        if (user == null)
            throw new DataNotFoundException("Return User Not Found");

        var inventory = await _returnRepsository.GetTheReturnInventoryByReturnId(returnId);
        if (inventory == null)
            throw new DataNotFoundException("Return Inventory Not Found");

        var product = await _returnRepsository.GetTheReturnProductByReturnId(returnId);
        if (product == null)
            throw new DataNotFoundException("Return Product Not Found");

        if (user.OrderItems == null)
            throw new DataNotFoundException("Return Order Item Not Found");

        if (user.OrderItems.Order == null)
            throw new DataNotFoundException("Order Not Found");

        if (user.OrderItems.Order.Address == null)
            throw new DataNotFoundException("Customer Address Not Found");

        if (product.OrderItems == null)
            throw new DataNotFoundException("Product Order Item Not Found");

        if (product.OrderItems.ProductVariant == null)
            throw new DataNotFoundException("Product Variant Not Found");

        if (inventory.OrderItems == null)
            throw new DataNotFoundException("Inventory Order Item Not Found");

        if (inventory.OrderItems.Inventory == null)
            throw new DataNotFoundException("Inventory Not Found");

        if (inventory.OrderItems.Inventory.Address == null)
            throw new DataNotFoundException("Inventory Address Not Found");

        var service = await CheckReturnShipmentServiceability(user,inventory,product);

        var shipment = await CreateReturnShipmentRecord(user,service);

        await CreateReturnShipmentItem(shipment.ShipmentId,user.OrderItemId);

        var trackingRemarks = await CreateReturnShipmentTracking(shipment.ShipmentId,user);

        return new ResponseCreateReturnShipmentDTO
        {
            ShipmentId = shipment.ShipmentId,
            OrderId = user.OrderItems.Order.OrderId,
            ShippingCharge = service.Rate,
            ExpectedDeliveryDate = shipment.ExpectedDeliveryDate,
            TrackingRemarks = trackingRemarks
        };
    }
    private async Task<CourierEstimateResponseDTO?> CheckReturnShipmentServiceability(Return user,Return inventory,Return product)
    {
        var customerAddress = user.OrderItems!.Order!.Address!;
        var inventoryAddress = inventory.OrderItems!.Inventory!.Address!;
        var productVariant = product.OrderItems!.ProductVariant!;

        var serviceabilityRequestDTO = new ServiceabilityRequestDTO
        {
            Cod = 0,
            Weight = productVariant.WeightInKgs,
            DeliveryPostcode = inventoryAddress.PinCode,
            PickupPostcode = customerAddress.PinCode
        };

        return await _shipRocketService.CheckServiceability(serviceabilityRequestDTO);
    }
    private async Task<Shipment> CreateReturnShipmentRecord(Return user,CourierEstimateResponseDTO service)
    {
        var order = user.OrderItems!.Order!;

        double estimatedDays = Convert.ToDouble(service.EstimatedDeliveryDays);

        var requestAddShipmentDTO = new RequestAddShipmentDTO
        {
            OrderId = order.OrderId,
            ExpectedDeliveryDate = DateTime.Now.AddDays(estimatedDays),
            ShippingCharge = service.Rate,
            PickupAddressId = order.AddressId
        };

        return await _shipmentService.CreateShipment(requestAddShipmentDTO);
    }
    private async Task CreateReturnShipmentItem(int shipmentId,int orderItemId)
    {
        await _shipmentService.CreateShipmentItems(shipmentId,orderItemId);
    }
    private async Task<string> CreateReturnShipmentTracking(int shipmentId,Return user)
    {
        var customerAddress = user.OrderItems!.Order!.Address!;

        var remarks = "Return shipment created from customer location";

        var trackingDTO = new RequestAddShipmentTrackingDTO
        {
            ShipmentId = shipmentId,
            ShipmentStatusId = 8,
            Location = customerAddress.City,
            Remarks = remarks
        };

        await _shipmentService.CreateShipmentTracking(trackingDTO);

        return remarks;
    }
}