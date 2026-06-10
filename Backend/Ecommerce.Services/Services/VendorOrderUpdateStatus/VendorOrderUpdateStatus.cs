
using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class VendorOrderService : IVendorOrderService
{
    private readonly IShipmentService _shipmentService;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IMapper _mapper;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IOrderValidation _orderValidation;
    private readonly IShipmentValidation _shipmentValidation;
    public VendorOrderService(IShipmentService shipmentService,IOrderValidation orderValidation, IOrderItemRepsository orderItemRepsository, IMapper mapper, IShipmentRepsository shipmentRepsository, IVendorUserValidation vendorUserValidation, IShipmentValidation shipmentValidation)
    {
        _shipmentService = shipmentService;
        _orderValidation = orderValidation;
        _orderItemRepsository = orderItemRepsository;
        _mapper = mapper;
        _shipmentRepsository = shipmentRepsository;
        _vendorUserValidation = vendorUserValidation;
        _shipmentValidation = shipmentValidation;
    }
    public async Task<List<OrderItemSummaryDto>> GetAllTheActiveOrder(int vendorId,int? status)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var Orders = await _orderValidation.ValidateGetOrderItemsByVendor(vendor.VendorId,status);
        return _mapper.Map<List<OrderItemSummaryDto>>(Orders);
    }
    public async Task<ResponseGetOrderItems> UpdateTheOrderStatus(int orderItemId)
    {
        var order = await _orderValidation.ValidateOrderItem(orderItemId);
        if(order!.Order!.OrderStatusId !=2)
        {
            throw new DataApprovalStatusException("Order cannot be updated");
        }
        order.OrderItemStatusId = 2;
        var updatedOrder = await _orderItemRepsository.Update(orderItemId, order);
        await CheckIfAllOrderForShipmetPacked(orderItemId);
        return _mapper.Map<ResponseGetOrderItems>(updatedOrder);
    }
    private async Task CheckIfAllOrderForShipmetPacked(int orderItemId)
    {
        var shipment = await _shipmentValidation.ValidateGetShipmentByOrderItemId(orderItemId);
        await _shipmentValidation.ValidateGetPendingPackedTheShipmentItemsByShipmentId(shipment.ShipmentId);

        string shipmentTrackingNumber = $"SHPTRACKI-" + shipment.ShipmentId.ToString();
        shipment.ShipmentStatusId = 4;
        shipment.TrackingNumber = shipmentTrackingNumber;
        await _shipmentRepsository.Update(shipment.ShipmentId, shipment);
        RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO = new RequestAddShipmentTrackingDTO();
        requestAddShipmentTrackingDTO.ShipmentId = shipment.ShipmentId;
        requestAddShipmentTrackingDTO.Location = "WareHouse";
        requestAddShipmentTrackingDTO.Remarks = "Shipment Created . Order Ready for pickup";
        requestAddShipmentTrackingDTO.ShipmentStatusId = shipment.ShipmentStatusId;
        await _shipmentService.CreateShipmentTracking(requestAddShipmentTrackingDTO);
    }
}