using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class VendorOrderService : IVendorOrderService
{
    private readonly EcommerceContext _ecommerceContext;
    private readonly ILogger<VendorOrderService> _logger;
    private readonly IShipmentService _shipmentService;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IMapper _mapper;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IOrderValidation _orderValidation;
    private readonly IShipmentValidation _shipmentValidation;
    private readonly ILogChanges _logChanges;
    private readonly INotificationService _notificationService;
    private readonly IAdminUserRepsository _adminUserRepsository;

    public VendorOrderService(
        EcommerceContext ecommerceContext,
        ILogger<VendorOrderService> logger,
        IShipmentService shipmentService,
        IOrderValidation orderValidation,
        IOrderItemRepsository orderItemRepsository,
        IMapper mapper,
        IShipmentRepsository shipmentRepsository,
        IVendorUserValidation vendorUserValidation,
        IShipmentValidation shipmentValidation,
        ILogChanges logChanges,
        INotificationService notificationService,
        IAdminUserRepsository adminUserRepsository)
    {
        _ecommerceContext = ecommerceContext;
        _shipmentService = shipmentService;
        _orderValidation = orderValidation;
        _orderItemRepsository = orderItemRepsository;
        _mapper = mapper;
        _logger = logger;
        _shipmentRepsository = shipmentRepsository;
        _vendorUserValidation = vendorUserValidation;
        _shipmentValidation = shipmentValidation;
        _logChanges = logChanges;
        _notificationService = notificationService;
        _adminUserRepsository = adminUserRepsository;
    }

    public async Task<List<OrderItemSummaryDto>> GetAllTheActiveOrder(int vendorId, int? status)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested active orders with Status {Status}", vendorId, status);
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var Orders = await _orderValidation.ValidateGetOrderItemsByVendor(vendor.VendorId, status);
        _logger.LogInformation("Returning {OrderCount} orders for VendorId {VendorId}", Orders.Count, vendor.VendorId);
        return _mapper.Map<List<OrderItemSummaryDto>>(Orders);
    }

    public async Task<ResponseGetOrderItems> UpdateTheOrderStatus(int orderItemId, int userId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Updating OrderItemId {OrderItemId} to Packed status", orderItemId);

            var order = await _orderValidation.ValidateOrderItem(orderItemId);
            if (order!.Order!.OrderStatusId != (int)OrderStatusEnum.Confirmed)
            {
                _logger.LogWarning("OrderItemId {OrderItemId} cannot be updated because OrderStatusId is {OrderStatusId}", orderItemId, order.Order.OrderStatusId);
                throw new DataApprovalStatusException("Order cannot be updated");
            }

            int previousOrderItemStatusId = order.OrderItemStatusId;
            order.OrderItemStatusId = (int)OrderItemStatusEnum.Packed;

            var updatedOrder = await _orderItemRepsository.Update(orderItemId, order);
            if (updatedOrder == null)
            {
                _logger.LogError("Failed to update OrderItemId {OrderItemId}", orderItemId);
                throw new DataRegistrationException("Updation of the order item failed");
            }
            _logger.LogInformation("OrderItemId {OrderItemId} updated successfully to Packed status", orderItemId);

            var orderItemLog = new LogChanges
            {
                TableName = nameof(OrderItems),
                RecordId = updatedOrder.OrderItemsId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"OrderItemsId={orderItemId}, OrderItemStatusId={previousOrderItemStatusId}",
                NewValue = $"OrderItemsId={updatedOrder.OrderItemsId}, OrderItemStatusId={updatedOrder.OrderItemStatusId}",
                UserId = userId,
                ChangedAt = DateTime.Now
            };

            var createdOrderItemLog = await _logChanges.Create(orderItemLog);
            if (createdOrderItemLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", orderItemLog.TableName, orderItemLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", orderItemLog.TableName, orderItemLog.RecordId);

            // Notify order admins that an order item was packed
            var orderAdminUserIds = await _adminUserRepsository.GetOrderAdminUserIds();
            _logger.LogInformation("Sending order item packed notification to {AdminCount} order admins for OrderItemId {OrderItemId}", orderAdminUserIds.Count, orderItemId);
            if (orderAdminUserIds.Count == 0)
            {
                _logger.LogWarning("No order admin users found to notify for OrderItemId {OrderItemId}", orderItemId);
            }
            foreach (var adminUserId in orderAdminUserIds)
            {
                await _notificationService.SendToUser(
                    adminUserId,
                    "Order Item Packed",
                    $"OrderItemId '{orderItemId}' has been packed by the vendor.",
                    notificationTypeId: (int)NotificationTypeEnum.OrderPacked,
                    referenceType: "OrderItem",
                    referenceId: orderItemId);
                _logger.LogInformation("Order item packed notification sent to order admin UserId {UserId}", adminUserId);
            }

            // Notify the customer who placed the order
            if (order.Order.UserId != 0)
            {
                _logger.LogInformation("Sending order item packed notification to UserId {UserId} for OrderItemId {OrderItemId}", order.Order.UserId, orderItemId);
                await _notificationService.SendToUser(
                    order.Order.UserId,
                    "Your Item Has Been Packed",
                    $"Your item from OrderId '{order.Order.OrderId}' has been packed and is being prepared for shipment.",
                    notificationTypeId: (int)NotificationTypeEnum.OrderPacked,
                    referenceType: "OrderItem",
                    referenceId: orderItemId);
                _logger.LogInformation("Order item packed notification sent to customer UserId {UserId}", order.Order.UserId);
            }
            else
            {
                _logger.LogWarning("OrderId {OrderId} has no associated UserId. Skipping customer notification", order.Order.OrderId);
            }

            await CheckIfAllOrderForShipmetPacked(orderItemId, userId);
            await transaction.CommitAsync();

            return _mapper.Map<ResponseGetOrderItems>(updatedOrder);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex,
                "Transaction failed while updating OrderItemId {OrderItemId}",
                orderItemId);

            throw;
        }
    }

    private async Task CheckIfAllOrderForShipmetPacked(int orderItemId, int userid)
    {
        _logger.LogDebug("Checking shipment completion for OrderItemId {OrderItemId}", orderItemId);

        var shipment = await _shipmentValidation.ValidateGetShipmentByOrderItemId(orderItemId);

        try
        {
            await _shipmentValidation.ValidateGetPendingPackedTheShipmentItemsByShipmentId(shipment.ShipmentId);
        }
        catch (DataAlreadyRegisteredException)
        {
            _logger.LogInformation("ShipmentId {ShipmentId} still has pending unpacked items. Skipping shipment-ready transition for now.", shipment.ShipmentId);
            return;
        }
        _logger.LogInformation("All shipment items packed for ShipmentId {ShipmentId}. Creating tracking number", shipment.ShipmentId);

        int previousShipmentStatusId = shipment.ShipmentStatusId;
        string shipmentTrackingNumber = $"SHPTRACKI-" + shipment.ShipmentId.ToString();
        shipment.ShipmentStatusId = (int)ShipmentStatusEnum.Ready_For_Pick_Up;
        shipment.TrackingNumber = shipmentTrackingNumber;

        var updatedShipment = await _shipmentRepsository.Update(shipment.ShipmentId, shipment);
        if (updatedShipment == null)
        {
            _logger.LogError("Failed to update ShipmentId {ShipmentId}", shipment.ShipmentId);
            throw new DataRegistrationException("Updation of the shipment failed");
        }
        _logger.LogInformation("ShipmentId {ShipmentId} updated with TrackingNumber {TrackingNumber}", updatedShipment.ShipmentId, shipmentTrackingNumber);

        var shipmentLog = new LogChanges
        {
            TableName = nameof(Shipment),
            RecordId = updatedShipment.ShipmentId,
            Actions = (int)AuditAction.Updated,
            OldValue = $"ShipmentId={shipment.ShipmentId}, ShipmentStatusId={previousShipmentStatusId}",
            NewValue = $"ShipmentId={updatedShipment.ShipmentId}, ShipmentStatusId={updatedShipment.ShipmentStatusId}, TrackingNumber={updatedShipment.TrackingNumber}",
            ChangedAt = DateTime.Now,
            UserId = userid
        };

        var createdShipmentLog = await _logChanges.Create(shipmentLog);
        if (createdShipmentLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);

        RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO = new RequestAddShipmentTrackingDTO();
        requestAddShipmentTrackingDTO.ShipmentId = shipment.ShipmentId;
        requestAddShipmentTrackingDTO.Location = "WareHouse";
        requestAddShipmentTrackingDTO.Remarks = "Shipment Created . Order Ready for pickup";
        requestAddShipmentTrackingDTO.ShipmentStatusId = shipment.ShipmentStatusId;
        await _shipmentService.CreateShipmentTracking(requestAddShipmentTrackingDTO);
        _logger.LogInformation("Shipment tracking created for ShipmentId {ShipmentId}", shipment.ShipmentId);

        // Notify shipment admins that the shipment is ready for pickup
        var shipmentAdminUserIds = await _adminUserRepsository.GetShipmentAdminUserIds();
        _logger.LogInformation("Sending shipment ready notification to {AdminCount} shipment admins for ShipmentId {ShipmentId}", shipmentAdminUserIds.Count, shipment.ShipmentId);
        if (shipmentAdminUserIds.Count == 0)
        {
            _logger.LogWarning("No shipment admin users found to notify for ShipmentId {ShipmentId}", shipment.ShipmentId);
        }
        foreach (var adminUserId in shipmentAdminUserIds)
        {
            await _notificationService.SendToUser(
                adminUserId,
                "Shipment Ready For Pickup",
                $"ShipmentId '{shipment.ShipmentId}' with TrackingNumber '{shipmentTrackingNumber}' is ready for pickup.",
                notificationTypeId: (int)NotificationTypeEnum.OrderReadyForPickup,
                referenceType: "Shipment",
                referenceId: shipment.ShipmentId);
            _logger.LogInformation("Shipment ready notification sent to shipment admin UserId {UserId}", adminUserId);
        }
    }
}