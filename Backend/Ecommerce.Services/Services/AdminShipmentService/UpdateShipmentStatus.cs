using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminShipmentService : IAdminShipmentService
{
    public async Task<ShipmentStatusResponseDTO> UpdateShimentStatus(ShipmentStatusRequestDTO shipmentStatusRequestDTO, int adminUserId)
    {
        _logger.LogInformation("UpdateShimentStatus started for ShipmentId {ShipmentId} by AdminUserId {AdminUserId}", shipmentStatusRequestDTO.ShipmentId, adminUserId);

        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var adminUser = await _adminUserValidation.ValidateShipmentAndCouponAdminUserByUserId(adminUserId);
            _logger.LogInformation("Admin user {AdminUserId} validated successfully", adminUserId);

            var shipment = await _shipmentRepsository.Get(shipmentStatusRequestDTO.ShipmentId);
            if (shipment == null)
            {
                _logger.LogWarning("Shipment not found for ShipmentId {ShipmentId}", shipmentStatusRequestDTO.ShipmentId);
                throw new DataNotFoundException("Shipment Not Found");
            }

            int previousShipmentStatusId = shipment.ShipmentStatusId;
            shipment.ShipmentStatusId = shipmentStatusRequestDTO.ShipmentStatusId;
            _logger.LogInformation("ShipmentId {ShipmentId} status changing from {OldStatus} to {NewStatus}", shipment.ShipmentId, previousShipmentStatusId, shipment.ShipmentStatusId);

            if (shipment.ShipmentStatusId == (int)ShipmentStatusEnum.Picked_Up)
            {
                _logger.LogInformation("ShipmentId {ShipmentId} marked as Picked_Up. Updating order items to Processed", shipment.ShipmentId);
                shipment.ShippedDate = DateTime.Now;
                await UpdateOrderItemStatus(shipment.ShipmentId, (int)OrderItemStatusEnum.Processed, adminUserId);
            }
            if (shipment.ShipmentStatusId == (int)ShipmentStatusEnum.Delivered && shipment.ShipmentTypeId == (int)ShipmentTypeEnum.Return)
            {
                _logger.LogInformation("ShipmentId {ShipmentId} is a Return shipment marked as Delivered. Marking as Returned", shipment.ShipmentId);
                shipment.DeliveryDate = DateTime.Now;
                shipment.ShipmentStatusId = (int)ShipmentStatusEnum.Returned;
                await ReturnUpdateOrderItemStatus(shipment.ShipmentId, (int)OrderItemStatusEnum.Returned, adminUserId);
            }
            if (shipment.ShipmentStatusId == (int)ShipmentStatusEnum.Delivered)
            {
                _logger.LogInformation("ShipmentId {ShipmentId} marked as Delivered. Updating order items to Delivered", shipment.ShipmentId);
                shipment.DeliveryDate = DateTime.Now;
                await UpdateOrderItemStatus(shipment.ShipmentId, (int)OrderItemStatusEnum.Delivered, adminUserId);
            }

            int oldShipmentStatus = shipment.ShipmentStatusId;
            var updatedShipment = await _shipmentRepsository.Update(shipment.ShipmentId, shipment);
            if (updatedShipment == null)
            {
                _logger.LogError("Failed to update ShipmentId {ShipmentId}", shipment.ShipmentId);
                throw new DataRegistrationException("Updation of the shipment failed");
            }
            _logger.LogInformation("ShipmentId {ShipmentId} updated successfully", updatedShipment.ShipmentId);

            var shipmentLog = new LogChanges
            {
                TableName = nameof(Shipment),
                RecordId = shipment.ShipmentId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ShipmentId={shipment.ShipmentId}, ShipmentStatusId={oldShipmentStatus}",
                NewValue = $"ShipmentId={updatedShipment.ShipmentId}, ShipmentStatusId={updatedShipment.ShipmentStatusId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(shipmentLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);

            RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO = new RequestAddShipmentTrackingDTO();
            requestAddShipmentTrackingDTO.ShipmentId = shipment.ShipmentId;
            requestAddShipmentTrackingDTO.Location = shipmentStatusRequestDTO.Location;
            requestAddShipmentTrackingDTO.Remarks = shipmentStatusRequestDTO.Remarks;
            requestAddShipmentTrackingDTO.ShipmentStatusId = shipment.ShipmentStatusId;

            _logger.LogInformation("Creating shipment tracking entry for ShipmentId {ShipmentId} at Location {Location}", shipment.ShipmentId, shipmentStatusRequestDTO.Location);
            await _shipmentService.CreateShipmentTracking(requestAddShipmentTrackingDTO);
            _logger.LogInformation("Shipment tracking entry created for ShipmentId {ShipmentId}", shipment.ShipmentId);

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for ShipmentId {ShipmentId}", shipment.ShipmentId);

            return _mapper.Map<ShipmentStatusResponseDTO>(updatedShipment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while updating ShipmentId {ShipmentId}", shipmentStatusRequestDTO.ShipmentId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for ShipmentId {ShipmentId}", shipmentStatusRequestDTO.ShipmentId);
            throw;
        }
    }

    private async Task<List<OrderItems>> UpdateOrderItemStatus(int shipmentId, int orderItemStatus, int adminUserId)
    {
        _logger.LogInformation("UpdateOrderItemStatus started for ShipmentId {ShipmentId}, TargetStatus {OrderItemStatus}", shipmentId, orderItemStatus);

        List<OrderItems> orderList = new List<OrderItems>();
        var orderItem = await _shipmentItemsRepsository.GetOrderItemsByShippingId(shipmentId);
        if (orderItem.Count == 0)
        {
            _logger.LogWarning("No order items found for ShipmentId {ShipmentId}", shipmentId);
            throw new DataNotFoundException("Order Item Not Found");
        }
        _logger.LogInformation("Found {Count} order items for ShipmentId {ShipmentId}", orderItem.Count, shipmentId);

        foreach (var order in orderItem)
        {
            int previousStatus = order.OrderItemStatusId;
            order.OrderItemStatusId = orderItemStatus;
            await _orderItemRepsository.Update(order.OrderItemsId, order);
            _logger.LogInformation("OrderItemsId {OrderItemsId} status changed from {OldStatus} to {NewStatus}", order.OrderItemsId, previousStatus, order.OrderItemStatusId);
            orderList.Add(order);

            var shipmentLog = new LogChanges
            {
                TableName = nameof(OrderItems),
                RecordId = order.OrderItemsId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"OrderItemsId={order.OrderItemsId}, OrderItemStatusId={previousStatus}",
                NewValue = $"OrderItemsId={order.OrderItemsId}, OrderItemStatusId={order.OrderItemStatusId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var orderAdminUserIds = await _adminUserRepsository.GetOrderAdminUserIds();

            foreach (var adminUser in orderAdminUserIds)
            {
                await _notificationService.SendToUser(
                    adminUser,
                   orderItemStatus == (int)OrderItemStatusEnum.Processed ? "Order Picked Up" : "Order Delivered",
                    $"Your order item '{order.ProductVariant.Product.ProductName}' has been {(orderItemStatus == (int)OrderItemStatusEnum.Processed ? "picked up" : "delivered")}.",
                    orderItemStatus == (int)OrderItemStatusEnum.Processed
                        ? (int)NotificationTypeEnum.OrderShipped
                        : (int)NotificationTypeEnum.OrderDelivered,
                    referenceType: "OrderItem",
                    referenceId: order.OrderItemsId);
                _logger.LogInformation("Order completed notification sent to order admin UserId {UserId}", adminUser);
            }



            await _notificationService.SendToUser(
                order.Order.UserId,
                orderItemStatus == (int)OrderItemStatusEnum.Processed ? "Order Picked Up" : "Order Delivered",
                $"Your order item '{order.ProductVariant.Product.ProductName}' has been {(orderItemStatus == (int)OrderItemStatusEnum.Processed ? "picked up" : "delivered")}.",
                orderItemStatus == (int)OrderItemStatusEnum.Processed
                    ? (int)NotificationTypeEnum.OrderShipped
                    : (int)NotificationTypeEnum.OrderDelivered,
                referenceType: "OrderItem",
                referenceId: order.OrderItemsId);

            var createdLog = await _logChanges.Create(shipmentLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);

            if (orderItemStatus == (int)OrderItemStatusEnum.Delivered)
            {
                var inventory = await _inventoryRepsository.Get(order.InventoryId);
                if (inventory == null)
                {
                    _logger.LogWarning("InventoryId {InventoryId} not found while releasing reserved stock for OrderItemsId {OrderItemsId}", order.InventoryId, order.OrderItemsId);
                    throw new DataNotFoundException("Inventory Not Found");
                }

                int previousReservedQuantity = inventory.ReservedQuantity;
                inventory.ReservedQuantity = inventory.ReservedQuantity - order.Quantity;
                
                var updatedInventory = await _inventoryRepsository.Update(inventory.InventoryId, inventory);
                if (updatedInventory == null)
                {
                    _logger.LogError("Failed to update InventoryId {InventoryId} while releasing reserved stock for OrderItemsId {OrderItemsId}", inventory.InventoryId, order.OrderItemsId);
                    throw new DataRegistrationException("Updation of the inventory failed");
                }
                _logger.LogInformation("InventoryId {InventoryId} ReservedQuantity released. {OldReserved} -> {NewReserved} for delivered OrderItemsId {OrderItemsId}",
                    updatedInventory.InventoryId, previousReservedQuantity, updatedInventory.ReservedQuantity, order.OrderItemsId);
                var inventoryLog = new LogChanges
                {
                    TableName = nameof(Inventory),
                    RecordId = updatedInventory.InventoryId,
                    Actions = (int)AuditAction.Updated,
                    OldValue = $"InventoryId={inventory.InventoryId}, ReservedQuantity={previousReservedQuantity}",
                    NewValue = $"InventoryId={updatedInventory.InventoryId}, ReservedQuantity={updatedInventory.ReservedQuantity}",
                    UserId = adminUserId,
                    ChangedAt = DateTime.Now
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

        _logger.LogInformation("Checking if all order items are shipped for OrderId {OrderId}", orderItem[0].OrderId);
        await CheckIfAllOrderItemsShipped(orderItem[0].OrderId, adminUserId);

        _logger.LogInformation("UpdateOrderItemStatus completed for ShipmentId {ShipmentId}", shipmentId);
        return orderList;
    }

    private async Task<List<OrderItems>> ReturnUpdateOrderItemStatus(int shipmentId, int orderItemStatus, int adminUserId)
    {
        _logger.LogInformation("ReturnUpdateOrderItemStatus started for ShipmentId {ShipmentId}, TargetStatus {OrderItemStatus}", shipmentId, orderItemStatus);

        List<OrderItems> orderList = new List<OrderItems>();
        var orderItem = await _shipmentItemsRepsository.GetOrderItemsByShippingId(shipmentId);
        if (orderItem.Count == 0)
        {
            _logger.LogWarning("No order items found for ShipmentId {ShipmentId}", shipmentId);
            throw new DataNotFoundException("Order Item Not Found");
        }
        _logger.LogInformation("Found {Count} order items for return processing on ShipmentId {ShipmentId}", orderItem.Count, shipmentId);

        foreach (var order in orderItem)
        {
            order.OrderItemStatusId = orderItemStatus;
            await _orderItemRepsository.Update(order.OrderItemsId, order);
            _logger.LogInformation("OrderItemsId {OrderItemsId} status set to {NewStatus}", order.OrderItemsId, order.OrderItemStatusId);

            var returns = await _returnRepsository.GetTheReturnProductByOrderItemId(order.OrderItemsId);
            if (returns == null)
            {
                _logger.LogWarning("No return record found for OrderItemsId {OrderItemsId}", order.OrderItemsId);
            }
            int previousReturnStatus = returns!.ReturnStatusId;
            returns.ReturnStatusId = (int)ReturnStatusEnum.Received;
            await _returnRepsository.Update(returns.ReturnId, returns);
            _logger.LogInformation("ReturnId {ReturnId} status changed from {OldStatus} to {NewStatus}", returns.ReturnId, previousReturnStatus, returns.ReturnStatusId);

            var shipmentLog = new LogChanges
            {
                TableName = nameof(Return),
                RecordId = returns.ReturnId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ReturnId={returns.ReturnId}, ReturnStatusId={previousReturnStatus}",
                NewValue = $"ReturnId={returns.ReturnId}, ReturnStatusId={returns.ReturnStatusId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(shipmentLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);

            orderList.Add(order);
        }

        _logger.LogInformation("ReturnUpdateOrderItemStatus completed for ShipmentId {ShipmentId}", shipmentId);
        return orderList;
    }

    private async Task<Order?> CheckIfAllOrderItemsShipped(int orderId, int adminUserId)
    {
        _logger.LogInformation("CheckIfAllOrderItemsShipped started for OrderId {OrderId}", orderId);

        var shipment = await _shipmentItemsRepsository.GetAllNotDeliveredOrderByOrderId(orderId);
        if (shipment.Count == 0)
        {
            _logger.LogInformation("All items delivered for OrderId {OrderId}. Marking order as Completed", orderId);

            var order = await _orderRepsository.Get(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for OrderId {OrderId}", orderId);
                throw new DataNotFoundException("Order Not Found");
            }
            int previousStatus = order.OrderStatusId;
            order.OrderStatusId = (int)OrderStatusEnum.Completed;
            await _orderRepsository.Update(orderId, order);
            _logger.LogInformation("OrderId {OrderId} status changed from {OldStatus} to {NewStatus}", order.OrderId, previousStatus, order.OrderStatusId);

            var shipmentLog = new LogChanges
            {
                TableName = nameof(Order),
                RecordId = order.OrderId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"OrderId={order.OrderId}, OrderStatusId={previousStatus}",
                NewValue = $"OrderId={order.OrderId}, OrderStatusId={order.OrderStatusId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var orderAdminUserIds = await _adminUserRepsository.GetOrderAdminUserIds();

            foreach (var adminUser in orderAdminUserIds)
            {
                await _notificationService.SendToUser(
                    adminUser,
                   "Order Completed",
                    $"Order #{order.OrderNumber} has been completed successfully. Thank you for shopping with us.",
                    notificationTypeId: (int)NotificationTypeEnum.OrderCompleted,
                    referenceType: "Order",
                    referenceId: order.OrderId);
                _logger.LogInformation("Order completed notification sent to order admin UserId {UserId}", adminUser);
            }

            await _notificationService.SendToUser(
                order.UserId,
                "Order Completed",
                $"Your order #{order.OrderNumber} has been completed successfully. Thank you for shopping with us.",
                notificationTypeId: (int)NotificationTypeEnum.OrderCompleted,
                referenceType: "Order",
                referenceId: order.OrderId);

            var createdLog = await _logChanges.Create(shipmentLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", shipmentLog.TableName, shipmentLog.RecordId);

            return order;
        }

        _logger.LogInformation("OrderId {OrderId} still has {Count} undelivered shipments. Not marking as Completed", orderId, shipment.Count);
        return null;
    }
}