using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class ShipmentService : IShipmentService
{
    private readonly ILogger<ShipmentService> _logger;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IShipmentItemsRepsository _shipmentItemsRepsository;
    private readonly IShipmentTrackingRepsository _shipmentTrackingRepsository;
    private readonly IMapper _mapper;
    public ShipmentService(ILogger<ShipmentService> logger, IShipmentTrackingRepsository shipmentTrackingRepsository, IMapper mapper, IShipmentItemsRepsository shipmentItemsRepsository, IShipmentRepsository shipmentRepsository)
    {
        _shipmentItemsRepsository = shipmentItemsRepsository;
        _shipmentRepsository = shipmentRepsository;
        _shipmentTrackingRepsository = shipmentTrackingRepsository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Shipment> CreateShipment(RequestAddShipmentDTO requestAddShipmentDTO)
    {
        _logger.LogInformation("Creating shipment for OrderId {OrderId}", requestAddShipmentDTO.OrderId);

        var shipment = _mapper.Map<Shipment>(requestAddShipmentDTO);
        var createdShipment = await _shipmentRepsository.Create(shipment);
        if (createdShipment == null)
        {
            _logger.LogWarning("Shipment creation failed for OrderId {OrderId}", requestAddShipmentDTO.OrderId);
            throw new DataNotFoundException("Shipment not created");
        }
        _logger.LogInformation("Shipment {ShipmentId} created successfully", createdShipment.ShipmentId);

        return createdShipment;
    }
    public async Task<ResponseAddShipmentTrackingDTO> CreateShipmentTracking(RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO)
    {
        _logger.LogInformation("Creating tracking for ShipmentId {ShipmentId} with StatusId {StatusId}", requestAddShipmentTrackingDTO.ShipmentId, requestAddShipmentTrackingDTO.ShipmentStatusId);
        var shipmentTracking = _mapper.Map<ShipmentTracking>(requestAddShipmentTrackingDTO);
        await _shipmentTrackingRepsository.Create(shipmentTracking);
        _logger.LogInformation("ShipmentTracking created successfully. ShipmentTrackingId: {ShipmentTrackingId}, ShipmentId: {ShipmentId}, StatusId: {StatusId}", shipmentTracking.ShipmentTrackingId, shipmentTracking.ShipmentId, shipmentTracking.ShipmentStatusId);
        return _mapper.Map<ResponseAddShipmentTrackingDTO>(shipmentTracking);
    }
    public async Task<ShipmentItems> CreateShipmentItems(int shipmentId, int orderitemid)
    {
        _logger.LogInformation("Adding OrderItemId {OrderItemId} to ShipmentId {ShipmentId}", orderitemid, shipmentId);
        ShipmentItems shipmentItems = new ShipmentItems();
        shipmentItems.OrderItemsId = orderitemid;
        shipmentItems.ShipmentId = shipmentId;
        await _shipmentItemsRepsository.Create(shipmentItems);
        _logger.LogInformation("ShipmentItem created successfully. ShipmentId: {ShipmentId}, OrderItemId: {OrderItemId}", shipmentId, orderitemid);
        return shipmentItems;
    }
}