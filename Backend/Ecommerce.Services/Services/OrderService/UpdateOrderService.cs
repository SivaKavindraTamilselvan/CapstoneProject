using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class OrderService : IOrderService
{
    // confirm the order status after payment confirmation
    // called directly
    public async Task ConfirmOrderStatus(int orderId, bool status)
    {
        _logger.LogInformation("Confirming OrderId {OrderId} status. PaymentSuccessful {Status}", orderId, status);

        var order = await _orderRepsository.Get(orderId);
        if (order == null)
        {
            _logger.LogWarning("OrderId {OrderId} not found", orderId);
            throw new DataNotFoundException("Order Not Found");
        }

        int previousOrderStatusId = order.OrderStatusId;
        order.OrderStatusId = status ? (int)OrderStatusEnum.Confirmed : (int)OrderStatusEnum.Failed;
        order.UpdatedAt = DateTime.Now;

        var updatedOrder = await _orderRepsository.Update(order.OrderId, order);
        if (updatedOrder == null)
        {
            _logger.LogError("Failed to update OrderId {OrderId}", order.OrderId);
            throw new DataRegistrationException("Updation of the order failed");
        }
        _logger.LogInformation("OrderId {OrderId} status changed from {OldStatus} to {NewStatus}", updatedOrder.OrderId, previousOrderStatusId, updatedOrder.OrderStatusId);

        var orderLog = new LogChanges
        {
            TableName = nameof(Order),
            RecordId = updatedOrder.OrderId,
            Actions = (int)AuditAction.Updated,
            OldValue = $"OrderId={order.OrderId}, OrderStatusId={previousOrderStatusId}",
            NewValue = $"OrderId={updatedOrder.OrderId}, OrderStatusId={updatedOrder.OrderStatusId}",
            ChangedAt = DateTime.Now,
            UserId = order.UserId
        };

        var createdOrderLog = await _logChanges.Create(orderLog);
        if (createdOrderLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", orderLog.TableName, orderLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", orderLog.TableName, orderLog.RecordId);

        if (status)
        {
            var orderItems = await _orderItemRepsository.GetOrderItemByOrderId(order.OrderId);
            _logger.LogInformation("Reserving inventory for {Count} order items on OrderId {OrderId}", orderItems.Count, order.OrderId);

            foreach (var orderItem in orderItems)
            {
                var inventory = await _inventoryValidation.ValidateInventory(orderItem.InventoryId);
                if (inventory == null)
                {
                    _logger.LogWarning("InventoryId {InventoryId} not found while reserving stock for OrderId {OrderId}", orderItem.InventoryId, order.OrderId);
                    throw new DataNotFoundException("Inventory Not Found");
                }

                int previousAvailableQuantity = inventory.AvailableQuantity;
                int previousReservedQuantity = inventory.ReservedQuantity;
                inventory.AvailableQuantity = inventory.AvailableQuantity - orderItem.Quantity;
                inventory.ReservedQuantity = inventory.ReservedQuantity + orderItem.Quantity;

                var updatedInventory = await _inventoryRepsository.Update(inventory.InventoryId, inventory);
                if (updatedInventory == null)
                {
                    _logger.LogError("Failed to update InventoryId {InventoryId} while reserving stock for OrderId {OrderId}", inventory.InventoryId, order.OrderId);
                    throw new DataRegistrationException("Updation of the inventory failed");
                }
                _logger.LogInformation("InventoryId {InventoryId} reserved. AvailableQuantity {OldAvailable}->{NewAvailable}, ReservedQuantity {OldReserved}->{NewReserved}",
                    updatedInventory.InventoryId, previousAvailableQuantity, updatedInventory.AvailableQuantity, previousReservedQuantity, updatedInventory.ReservedQuantity);

                var inventoryLog = new LogChanges
                {
                    TableName = nameof(Inventory),
                    RecordId = updatedInventory.InventoryId,
                    Actions = (int)AuditAction.Updated,
                    OldValue = $"InventoryId={inventory.InventoryId}, AvailableQuantity={previousAvailableQuantity}, ReservedQuantity={previousReservedQuantity}",
                    NewValue = $"InventoryId={updatedInventory.InventoryId}, AvailableQuantity={updatedInventory.AvailableQuantity}, ReservedQuantity={updatedInventory.ReservedQuantity}",
                    ChangedAt = DateTime.Now,
                    UserId = order.UserId
                };

                var createdInventoryLog = await _logChanges.Create(inventoryLog);
                if (createdInventoryLog == null)
                {
                    _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", inventoryLog.TableName, inventoryLog.RecordId);
                    throw new DataRegistrationException("Audit log creation failed.");
                }
                _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", inventoryLog.TableName, inventoryLog.RecordId);
            }
        }
        else
        {
            _logger.LogInformation("OrderId {OrderId} payment failed. No inventory reservation performed", order.OrderId);
        }

        var shipment = await _shipmentRepsository.GetShipmentByOrderId(order.OrderId);
        _logger.LogInformation("Found {ShipmentCount} shipments for OrderId {OrderId} to update", shipment.Count, order.OrderId);

        foreach (var item in shipment)
        {
            int previousShipmentStatusId = item.ShipmentStatusId;
            item.ShipmentStatusId = 2;

            var updatedShipment = await _shipmentRepsository.Update(item.ShipmentId, item);
            if (updatedShipment == null)
            {
                _logger.LogError("Failed to update ShipmentId {ShipmentId}", item.ShipmentId);
                throw new DataRegistrationException("Updation of the shipment failed");
            }
            _logger.LogInformation("ShipmentId {ShipmentId} status changed from {OldStatus} to {NewStatus}", updatedShipment.ShipmentId, previousShipmentStatusId, updatedShipment.ShipmentStatusId);

            var shipmentLog = new LogChanges
            {
                TableName = nameof(Shipment),
                RecordId = updatedShipment.ShipmentId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ShipmentId={item.ShipmentId}, ShipmentStatusId={previousShipmentStatusId}",
                NewValue = $"ShipmentId={updatedShipment.ShipmentId}, ShipmentStatusId={updatedShipment.ShipmentStatusId}",
                ChangedAt = DateTime.Now,
                UserId = order.UserId
            };

            var createdShipmentLog = await _logChanges.Create(shipmentLog);
            if (createdShipmentLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);

            RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO = new RequestAddShipmentTrackingDTO();
            requestAddShipmentTrackingDTO.ShipmentId = item.ShipmentId;
            requestAddShipmentTrackingDTO.ShipmentStatusId = item.ShipmentStatusId;
            requestAddShipmentTrackingDTO.Location = "Warehouse";
            requestAddShipmentTrackingDTO.Remarks = "Order Successfully. Items is Warehouse";
            await _shipmentService.CreateShipmentTracking(requestAddShipmentTrackingDTO);
            _logger.LogInformation("Shipment tracking created for ShipmentId {ShipmentId}", item.ShipmentId);
        }

        _logger.LogInformation("ConfirmOrderStatus completed for OrderId {OrderId}", orderId);
    }
}