using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class ShipmentService : IShipmentService
{
    private readonly IOrderRepsository _orderRepsository;
    private readonly ILogger<ShipmentService> _logger;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IShipmentItemsRepsository _shipmentItemsRepsository;
    private readonly IShipmentTrackingRepsository _shipmentTrackingRepsository;
    private readonly IMapper _mapper;
    private readonly ILogChanges _logChanges;
    private readonly EcommerceContext _ecommerceContext;

    public ShipmentService(
        IOrderRepsository orderRepsository,
        ILogger<ShipmentService> logger,
        IShipmentTrackingRepsository shipmentTrackingRepsository,
        IMapper mapper,
        IShipmentItemsRepsository shipmentItemsRepsository,
        IShipmentRepsository shipmentRepsository,
        ILogChanges logChanges,
        EcommerceContext ecommerceContext)
    {
        _orderRepsository = orderRepsository;
        _shipmentItemsRepsository = shipmentItemsRepsository;
        _shipmentRepsository = shipmentRepsository;
        _shipmentTrackingRepsository = shipmentTrackingRepsository;
        _mapper = mapper;
        _logger = logger;
        _logChanges = logChanges;
        _ecommerceContext = ecommerceContext;
    }

    public async Task<Shipment> CreateShipment(RequestAddShipmentDTO requestAddShipmentDTO)
    {

        _logger.LogInformation("Creating shipment for OrderId {OrderId}", requestAddShipmentDTO.OrderId);

        var shipment = _mapper.Map<Shipment>(requestAddShipmentDTO);
        if (shipment == null)
        {
            _logger.LogError("Failed to map RequestAddShipmentDTO to Shipment entity for OrderId {OrderId}", requestAddShipmentDTO.OrderId);
            throw new NullReferenceException("Shipment mapping failed");
        }

        var createdShipment = await _shipmentRepsository.Create(shipment);
        if (createdShipment == null)
        {
            _logger.LogWarning("Shipment creation failed for OrderId {OrderId}", requestAddShipmentDTO.OrderId);
            throw new DataNotFoundException("Shipment not created");
        }
        _logger.LogInformation("Shipment {ShipmentId} created successfully", createdShipment.ShipmentId);

        var orderdetail = await _orderRepsository.Get(requestAddShipmentDTO.OrderId);

        var shipmentLog = new LogChanges
        {
            TableName = nameof(Shipment),
            RecordId = createdShipment.ShipmentId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"ShipmentId={createdShipment.ShipmentId}, OrderId={createdShipment.OrderId}, ShipmentStatusId={createdShipment.ShipmentStatusId}, ShipmentTypeId={createdShipment.ShipmentTypeId}",
            ChangedAt = DateTime.Now,
            UserId = orderdetail.UserId
        };

        var createdLog = await _logChanges.Create(shipmentLog);
        if (createdLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);


        _logger.LogInformation("Transaction committed successfully for ShipmentId {ShipmentId}", createdShipment.ShipmentId);

        return createdShipment;

    }

    public async Task<ResponseAddShipmentTrackingDTO> CreateShipmentTracking(RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO)
    {

        _logger.LogInformation("Creating tracking for ShipmentId {ShipmentId} with StatusId {StatusId}", requestAddShipmentTrackingDTO.ShipmentId, requestAddShipmentTrackingDTO.ShipmentStatusId);

        var shipmentTracking = _mapper.Map<ShipmentTracking>(requestAddShipmentTrackingDTO);
        if (shipmentTracking == null)
        {
            _logger.LogError("Failed to map RequestAddShipmentTrackingDTO to ShipmentTracking entity for ShipmentId {ShipmentId}", requestAddShipmentTrackingDTO.ShipmentId);
            throw new NullReferenceException("Shipment tracking mapping failed");
        }

        var createdShipmentTracking = await _shipmentTrackingRepsository.Create(shipmentTracking);
        if (createdShipmentTracking == null)
        {
            _logger.LogError("Failed to create ShipmentTracking for ShipmentId {ShipmentId}", requestAddShipmentTrackingDTO.ShipmentId);
            throw new DataRegistrationException("Shipment tracking creation failed");
        }
        _logger.LogInformation("ShipmentTracking created successfully. ShipmentTrackingId: {ShipmentTrackingId}, ShipmentId: {ShipmentId}, StatusId: {StatusId}", createdShipmentTracking.ShipmentTrackingId, createdShipmentTracking.ShipmentId, createdShipmentTracking.ShipmentStatusId);

        var shipmentdetails = await _shipmentRepsository.Get(requestAddShipmentTrackingDTO.ShipmentId);
        var orderdetail = await _orderRepsository.Get(shipmentdetails.OrderId);

        var trackingLog = new LogChanges
        {
            TableName = nameof(ShipmentTracking),
            RecordId = createdShipmentTracking.ShipmentTrackingId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"ShipmentTrackingId={createdShipmentTracking.ShipmentTrackingId}, ShipmentId={createdShipmentTracking.ShipmentId}, ShipmentStatusId={createdShipmentTracking.ShipmentStatusId}, Location={createdShipmentTracking.Location}",
            ChangedAt = DateTime.Now,
            UserId = orderdetail.UserId
        };

        var createdLog = await _logChanges.Create(trackingLog);
        if (createdLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", trackingLog.TableName, trackingLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", trackingLog.TableName, trackingLog.RecordId);


        _logger.LogInformation("Transaction committed successfully for ShipmentTrackingId {ShipmentTrackingId}", createdShipmentTracking.ShipmentTrackingId);

        return _mapper.Map<ResponseAddShipmentTrackingDTO>(createdShipmentTracking);

    }

    public async Task<ShipmentItems> CreateShipmentItems(int shipmentId, int orderitemid)
    {

        _logger.LogInformation("Adding OrderItemId {OrderItemId} to ShipmentId {ShipmentId}", orderitemid, shipmentId);

        ShipmentItems shipmentItems = new ShipmentItems();
        shipmentItems.OrderItemsId = orderitemid;
        shipmentItems.ShipmentId = shipmentId;

        var createdShipmentItems = await _shipmentItemsRepsository.Create(shipmentItems);
        if (createdShipmentItems == null)
        {
            _logger.LogError("Failed to create ShipmentItems for ShipmentId {ShipmentId}, OrderItemId {OrderItemId}", shipmentId, orderitemid);
            throw new DataRegistrationException("Shipment item creation failed");
        }
        _logger.LogInformation("ShipmentItem created successfully. ShipmentId: {ShipmentId}, OrderItemId: {OrderItemId}", createdShipmentItems.ShipmentId, createdShipmentItems.OrderItemsId);

        var shipmentdetails = await _shipmentRepsository.Get(shipmentId);
        var orderdetail = await _orderRepsository.Get(shipmentdetails.OrderId);

        var shipmentItemLog = new LogChanges
        {
            TableName = nameof(ShipmentItems),
            RecordId = createdShipmentItems.ShipmentItemsId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"ShipmentItemsId={createdShipmentItems.ShipmentItemsId}, ShipmentId={createdShipmentItems.ShipmentId}, OrderItemsId={createdShipmentItems.OrderItemsId}",
            ChangedAt = DateTime.Now,
            UserId = orderdetail.UserId
        };

        var createdLog = await _logChanges.Create(shipmentItemLog);
        if (createdLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", shipmentItemLog.TableName, shipmentItemLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", shipmentItemLog.TableName, shipmentItemLog.RecordId);

        _logger.LogInformation("Transaction committed successfully for ShipmentItemsId {ShipmentItemsId}", createdShipmentItems.ShipmentItemsId);

        return createdShipmentItems;

    }
}