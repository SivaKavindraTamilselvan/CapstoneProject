
using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class VendorOrderService : IVendorOrderService
{
    private readonly ILogger<VendorOrderService> _logger;
    private readonly IShipmentService _shipmentService;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IMapper _mapper;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IOrderValidation _orderValidation;
    private readonly IShipmentValidation _shipmentValidation;
    public VendorOrderService(ILogger<VendorOrderService> logger,IShipmentService shipmentService, IOrderValidation orderValidation, IOrderItemRepsository orderItemRepsository, IMapper mapper, IShipmentRepsository shipmentRepsository, IVendorUserValidation vendorUserValidation, IShipmentValidation shipmentValidation)
    {
        _shipmentService = shipmentService;
        _orderValidation = orderValidation;
        _orderItemRepsository = orderItemRepsository;
        _mapper = mapper;
        _logger = logger;
        _shipmentRepsository = shipmentRepsository;
        _vendorUserValidation = vendorUserValidation;
        _shipmentValidation = shipmentValidation;
    }
    public async Task<List<OrderItemSummaryDto>> GetAllTheActiveOrder(int vendorId, int? status)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested active orders with Status {Status}",vendorId,status);
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var Orders = await _orderValidation.ValidateGetOrderItemsByVendor(vendor.VendorId, status);
        _logger.LogInformation("Returning {OrderCount} orders for VendorId {VendorId}",Orders.Count,vendor.VendorId);
        return _mapper.Map<List<OrderItemSummaryDto>>(Orders);
    }
    public async Task<ResponseGetOrderItems> UpdateTheOrderStatus(int orderItemId)
    {
        _logger.LogInformation("Updating OrderItemId {OrderItemId} to Packed status",orderItemId);
        var order = await _orderValidation.ValidateOrderItem(orderItemId);
        if (order!.Order!.OrderStatusId != 2)
        {
            _logger.LogWarning("OrderItemId {OrderItemId} cannot be updated because OrderStatusId is {OrderStatusId}",orderItemId,order.Order.OrderStatusId);
            throw new DataApprovalStatusException("Order cannot be updated");
        }
        order.OrderItemStatusId = 2;
        var updatedOrder = await _orderItemRepsository.Update(orderItemId, order);
        _logger.LogInformation("OrderItemId {OrderItemId} updated successfully to Packed status",orderItemId);

        await CheckIfAllOrderForShipmetPacked(orderItemId);
        return _mapper.Map<ResponseGetOrderItems>(updatedOrder);
    }
    private async Task CheckIfAllOrderForShipmetPacked(int orderItemId)
    {
        _logger.LogDebug("Checking shipment completion for OrderItemId {OrderItemId}",orderItemId);
        var shipment = await _shipmentValidation.ValidateGetShipmentByOrderItemId(orderItemId);
        await _shipmentValidation.ValidateGetPendingPackedTheShipmentItemsByShipmentId(shipment.ShipmentId);
        _logger.LogInformation("All shipment items packed for ShipmentId {ShipmentId}. Creating tracking number",shipment.ShipmentId);
        string shipmentTrackingNumber = $"SHPTRACKI-" + shipment.ShipmentId.ToString();
        shipment.ShipmentStatusId = 4;
        shipment.TrackingNumber = shipmentTrackingNumber;
        await _shipmentRepsository.Update(shipment.ShipmentId, shipment);
        _logger.LogInformation("ShipmentId {ShipmentId} updated with TrackingNumber {TrackingNumber}",shipment.ShipmentId,shipmentTrackingNumber);
        RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO = new RequestAddShipmentTrackingDTO();
        requestAddShipmentTrackingDTO.ShipmentId = shipment.ShipmentId;
        requestAddShipmentTrackingDTO.Location = "WareHouse";
        requestAddShipmentTrackingDTO.Remarks = "Shipment Created . Order Ready for pickup";
        requestAddShipmentTrackingDTO.ShipmentStatusId = shipment.ShipmentStatusId;
        await _shipmentService.CreateShipmentTracking(requestAddShipmentTrackingDTO);
        _logger.LogInformation("Shipment tracking created for ShipmentId {ShipmentId}",shipment.ShipmentId);
    }
}