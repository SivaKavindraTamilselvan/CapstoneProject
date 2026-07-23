using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class CancelService : ICancelService
{
    public async Task<ResponseCancelDTO> RequestCancel(RequestCancelDTO requestCancelDTO, int userId)
    {
        _logger.LogInformation("UserId {UserId} requested cancellation for OrderItemId {OrderItemId}, CancelQuantity {CancelQuantity}", userId, requestCancelDTO.OrderItemId, requestCancelDTO.CancelQuantity);

        var order = await _orderValidation.ValidateOrderItem(requestCancelDTO.OrderItemId);
        if (order == null)
        {
            _logger.LogWarning("OrderItemId {OrderItemId} not found", requestCancelDTO.OrderItemId);
            throw new DataNotFoundException("Order Item Not Found");
        }

        var orderDetails = await _orderItemRepsository.GetOrderItemByOrderItemId(order.OrderItemsId);
        if (orderDetails == null)
        {
            _logger.LogWarning("Order details not found for OrderItemId {OrderItemId}", order.OrderItemsId);
            throw new DataNotFoundException("Order Item Details Not Found");
        }

        if (orderDetails!.Order!.UserId != userId)
        {
            _logger.LogWarning("UserId {UserId} attempted to cancel OrderItemId {OrderItemId} belonging to another user", userId, order.OrderItemsId);
            throw new UnauthorizationException("User Cannot access other user order items");
        }

        if (requestCancelDTO.CancelQuantity > order.Quantity)
        {
            _logger.LogWarning("CancelQuantity {CancelQuantity} exceeds OrderQuantity {OrderQuantity} for OrderItemId {OrderItemId}", requestCancelDTO.CancelQuantity, order.Quantity, order.OrderItemsId);
            throw new DataApprovalStatusException("Quantity Higher Than the Order Quantity Is Not Possible");
        }

        if (order.OrderItemStatusId != (int)OrderItemStatusEnum.Pending && order.OrderItemStatusId != (int)OrderItemStatusEnum.Packed)
        {
            _logger.LogWarning("OrderItemId {OrderItemId} cannot be cancelled. Current OrderItemStatusId {OrderItemStatusId}", order.OrderItemsId, order.OrderItemStatusId);
            throw new DataApprovalStatusException("Order Already Shipped. Cannot be cancelled");
        }

        var orderItemAmount = order.Quantity * order.UnitPrice - order.Discount;
        var ConvenienceFee = orderItemAmount * 0.05m;
        var refundAmount = orderItemAmount - ConvenienceFee;
        _logger.LogInformation("Calculated refund for OrderItemId {OrderItemId}: Amount {Amount}, ConvenienceFee {Fee}, RefundAmount {RefundAmount}", order.OrderItemsId, orderItemAmount, ConvenienceFee, refundAmount);

        var cancelledOrderItemId = await UpdateOrderItemStatus(order.OrderItemsId, requestCancelDTO.CancelQuantity, userId);

        var cancel = _mapper.Map<Cancel>(requestCancelDTO);
        if (cancel == null)
        {
            _logger.LogError("Failed to map RequestCancelDTO to Cancel entity for OrderItemId {OrderItemId}", order.OrderItemsId);
            throw new NullReferenceException("Cancel mapping failed");
        }
        cancel.CancelledDate = DateTime.Now;
        cancel.OrderItemId = cancelledOrderItemId;
        cancel.CancelStatusId = (int)CancelStatusEnum.Approved;
        cancel.ConvenienceFee = ConvenienceFee;

        var createdCancel = await _cancelRepsository.Create(cancel);
        if (createdCancel == null)
        {
            _logger.LogError("Failed to create Cancel record for OrderItemId {OrderItemId}", cancelledOrderItemId);
            throw new DataRegistrationException("Cancel creation failed");
        }
        _logger.LogInformation("Cancel {CancelId} created for OrderItemId {OrderItemId}", createdCancel.CancelId, cancelledOrderItemId);

        var cancelLog = new LogChanges
        {
            TableName = nameof(Cancel),
            RecordId = createdCancel.CancelId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"CancelId={createdCancel.CancelId}, OrderItemId={createdCancel.OrderItemId}, CancelStatusId={createdCancel.CancelStatusId}, ConvenienceFee={createdCancel.ConvenienceFee}",
            UserId = userId,
            ChangedAt = DateTime.Now
        };

        var createdCancelLog = await _logChanges.Create(cancelLog);
        if (createdCancelLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", cancelLog.TableName, cancelLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", cancelLog.TableName, cancelLog.RecordId);

        await UpdateOrder(order.OrderId);
        await UpdateInventory(order.InventoryId, requestCancelDTO.CancelQuantity, userId);
        var details = await _orderService.GetOrderInvoiceData(order.OrderId);
        if (details.PaymentMethod != "Cash On Delivery")
        {
            await CreateRefund(order.OrderItemsId, 1, createdCancel.CancelId, refundAmount, userId);
        }

        var vendorId = orderDetails.ProductVariant?.Product?.VendorId;
        if (vendorId != null && vendorId != 0)
        {
            var ownerUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendorId.Value);
            if (ownerUser != null)
            {
                await _notificationService.SendToUser(
                    ownerUser.UserId,
                    "Order Item Cancelled",
                    $"OrderItemId '{order.OrderItemsId}' has been cancelled by the customer. Quantity: {requestCancelDTO.CancelQuantity}",
                    notificationTypeId: (int)NotificationTypeEnum.OrderCancelled,
                    referenceType: "OrderItem",
                    referenceId: order.OrderItemsId);
                _logger.LogInformation("Cancellation notification sent to vendor owner UserId {UserId}", ownerUser.UserId);
            }
            else
            {
                _logger.LogWarning("No owner vendor user found for VendorId {VendorId}. Skipping vendor notification", vendorId);
            }
        }
        else
        {
            _logger.LogWarning("No VendorId resolved for OrderItemId {OrderItemId}. Skipping vendor notification", order.OrderItemsId);
        }

        var orderAdminUserIds = await _adminUserRepsository.GetOrderAdminUserIds();
        _logger.LogInformation("Sending cancellation notification to {AdminCount} order admins for OrderItemId {OrderItemId}", orderAdminUserIds.Count, order.OrderItemsId);
        if (orderAdminUserIds.Count == 0)
        {
            _logger.LogWarning("No order admin users found to notify for OrderItemId {OrderItemId}", order.OrderItemsId);
        }
        foreach (var adminUserId in orderAdminUserIds)
        {
            await _notificationService.SendToUser(
                adminUserId,
                "Order Item Cancelled",
                $"OrderItemId '{order.OrderItemsId}' has been cancelled by the customer. Quantity: {requestCancelDTO.CancelQuantity}",
                notificationTypeId: (int)NotificationTypeEnum.OrderCancelled,
                referenceType: "OrderItem",
                referenceId: order.OrderItemsId);
            _logger.LogInformation("Cancellation notification sent to order admin UserId {UserId}", adminUserId);
        }

        await _notificationService.SendToUser(
            userId,
            "Your Order Item Has Been Cancelled",
            $"Your cancellation request for OrderItemId '{order.OrderItemsId}' has been processed. A refund of {refundAmount:C} will be issued.",
            notificationTypeId: (int)NotificationTypeEnum.OrderCancelled,
            referenceType: "OrderItem",
            referenceId: order.OrderItemsId);
        _logger.LogInformation("Cancellation confirmation notification sent to customer UserId {UserId}", userId);

        return _mapper.Map<ResponseCancelDTO>(createdCancel);
    }

    private async Task UpdateInventory(int inventoryId, int Quantity, int userid)
    {
        _logger.LogInformation("Updating inventory InventoryId {InventoryId} for cancelled Quantity {Quantity}", inventoryId, Quantity);

        var inventory = await _inventoryValidation.ValidateInventory(inventoryId);
        if (inventory == null)
        {
            _logger.LogWarning("InventoryId {InventoryId} not found", inventoryId);
            throw new DataNotFoundException("Inventory Not Found");
        }

        int previousAvailableQuantity = inventory.AvailableQuantity;
        int previousReservedQuantity = inventory.ReservedQuantity;

        inventory.AvailableQuantity = inventory.AvailableQuantity + Quantity;
        inventory.ReservedQuantity = inventory.ReservedQuantity - Quantity;
        inventory.UpdatedAt = DateTime.Now;

        var updatedInventory = await _inventoryRepsository.Update(inventory.InventoryId, inventory);
        if (updatedInventory == null)
        {
            _logger.LogError("Failed to update InventoryId {InventoryId}", inventoryId);
            throw new DataRegistrationException("Updation of the inventory failed");
        }
        _logger.LogInformation("InventoryId {InventoryId} updated. AvailableQuantity {OldAvailable}->{NewAvailable}, ReservedQuantity {OldReserved}->{NewReserved}",
            updatedInventory.InventoryId, previousAvailableQuantity, updatedInventory.AvailableQuantity, previousReservedQuantity, updatedInventory.ReservedQuantity);

        var inventoryLog = new LogChanges
        {
            TableName = nameof(Inventory),
            RecordId = updatedInventory.InventoryId,
            Actions = (int)AuditAction.Updated,
            OldValue = $"InventoryId={inventoryId}, AvailableQuantity={previousAvailableQuantity}, ReservedQuantity={previousReservedQuantity}",
            NewValue = $"InventoryId={updatedInventory.InventoryId}, AvailableQuantity={updatedInventory.AvailableQuantity}, ReservedQuantity={updatedInventory.ReservedQuantity}",
            ChangedAt = DateTime.Now,
            UserId = userid
        };

        var createdLog = await _logChanges.Create(inventoryLog);
        if (createdLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", inventoryLog.TableName, inventoryLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", inventoryLog.TableName, inventoryLog.RecordId);
    }

    private async Task<int> UpdateOrderItemStatus(int orderItemId, int cancelQuantity, int userId)
    {
        _logger.LogInformation("Updating OrderItemId {OrderItemId} status for CancelQuantity {CancelQuantity}", orderItemId, cancelQuantity);

        var orderItem = await _orderValidation.ValidateOrderItem(orderItemId);
        if (orderItem == null)
        {
            _logger.LogWarning("OrderItemId {OrderItemId} not found", orderItemId);
            throw new DataNotFoundException("Order Item Not Found");
        }

        if (orderItem.Quantity == cancelQuantity)
        {
            int previousStatusId = orderItem.OrderItemStatusId;
            orderItem.OrderItemStatusId = (int)OrderItemStatusEnum.Cancelled;

            var updatedOrderItem = await _orderItemRepsository.Update(orderItem.OrderItemsId, orderItem);
            if (updatedOrderItem == null)
            {
                _logger.LogError("Failed to update OrderItemId {OrderItemId}", orderItem.OrderItemsId);
                throw new DataRegistrationException("Updation of the order item failed");
            }
            _logger.LogInformation("OrderItemId {OrderItemId} fully cancelled. Status {OldStatus}->{NewStatus}", updatedOrderItem.OrderItemsId, previousStatusId, updatedOrderItem.OrderItemStatusId);

            var fullCancelLog = new LogChanges
            {
                TableName = nameof(OrderItems),
                RecordId = updatedOrderItem.OrderItemsId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"OrderItemsId={orderItem.OrderItemsId}, OrderItemStatusId={previousStatusId}",
                NewValue = $"OrderItemsId={updatedOrderItem.OrderItemsId}, OrderItemStatusId={updatedOrderItem.OrderItemStatusId}",
                ChangedAt = DateTime.Now,
                UserId = userId
            };
            var createdFullCancelLog = await _logChanges.Create(fullCancelLog);
            if (createdFullCancelLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", fullCancelLog.TableName, fullCancelLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", fullCancelLog.TableName, fullCancelLog.RecordId);

            return updatedOrderItem.OrderItemsId;
        }

        int previousQuantity = orderItem.Quantity;
        orderItem.Quantity -= cancelQuantity;

        var updatedPartialOrderItem = await _orderItemRepsository.Update(orderItem.OrderItemsId, orderItem);
        if (updatedPartialOrderItem == null)
        {
            _logger.LogError("Failed to update OrderItemId {OrderItemId}", orderItem.OrderItemsId);
            throw new DataRegistrationException("Updation of the order item failed");
        }
        _logger.LogInformation("OrderItemId {OrderItemId} partially cancelled. Quantity {OldQuantity}->{NewQuantity}", updatedPartialOrderItem.OrderItemsId, previousQuantity, updatedPartialOrderItem.Quantity);

        var partialCancelLog = new LogChanges
        {
            TableName = nameof(OrderItems),
            RecordId = updatedPartialOrderItem.OrderItemsId,
            Actions = (int)AuditAction.Updated,
            OldValue = $"OrderItemsId={orderItem.OrderItemsId}, Quantity={previousQuantity}",
            NewValue = $"OrderItemsId={updatedPartialOrderItem.OrderItemsId}, Quantity={updatedPartialOrderItem.Quantity}",
            ChangedAt = DateTime.Now,
            UserId = userId
        };
        var createdPartialCancelLog = await _logChanges.Create(partialCancelLog);
        if (createdPartialCancelLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", partialCancelLog.TableName, partialCancelLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", partialCancelLog.TableName, partialCancelLog.RecordId);

        var cancelledItem = new OrderItems
        {
            OrderId = orderItem.OrderId,
            ProductVariantId = orderItem.ProductVariantId,
            InventoryId = orderItem.InventoryId,
            Quantity = cancelQuantity,
            UnitPrice = orderItem.UnitPrice,
            Discount = (orderItem.Discount / (orderItem.Quantity + cancelQuantity)) * cancelQuantity,
            OrderItemStatusId = (int)OrderItemStatusEnum.Cancelled,
        };

        var createdCancelledItem = await _orderItemRepsository.Create(cancelledItem);
        if (createdCancelledItem == null)
        {
            _logger.LogError("Failed to create split cancelled OrderItem for original OrderItemId {OrderItemId}", orderItem.OrderItemsId);
            throw new DataRegistrationException("Cancelled order item creation failed");
        }
        _logger.LogInformation("Split cancelled OrderItemId {NewOrderItemId} created from OrderItemId {OriginalOrderItemId}, Quantity {Quantity}", createdCancelledItem.OrderItemsId, orderItem.OrderItemsId, cancelQuantity);

        var splitLog = new LogChanges
        {
            TableName = nameof(OrderItems),
            RecordId = createdCancelledItem.OrderItemsId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"OrderItemsId={createdCancelledItem.OrderItemsId}, OrderId={createdCancelledItem.OrderId}, Quantity={createdCancelledItem.Quantity}, OrderItemStatusId={createdCancelledItem.OrderItemStatusId}",
            ChangedAt = DateTime.Now,
            UserId = userId
        };
        var createdSplitLog = await _logChanges.Create(splitLog);
        if (createdSplitLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", splitLog.TableName, splitLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", splitLog.TableName, splitLog.RecordId);

        return createdCancelledItem.OrderItemsId;
    }

    private async Task UpdateOrder(int orderId)
    {
        _logger.LogInformation("Checking OrderId {OrderId} to determine if fully cancelled", orderId);

        var orderItems = await _orderItemRepsository.GetCancelledOrderItemsByOrderId(orderId);
        var order = await _orderValidation.ValidateOrder(orderId);
        if (order == null)
        {
            _logger.LogWarning("OrderId {OrderId} not found", orderId);
            throw new DataNotFoundException("Order Not Found");
        }

        if (orderItems.Count == 0)
        {
            int previousStatusId = order.OrderStatusId;
            order.OrderStatusId = (int)OrderStatusEnum.Cancelled;
            order.UpdatedAt = DateTime.Now;

            var updatedOrder = await _orderRepsository.Update(orderId, order);
            if (updatedOrder == null)
            {
                _logger.LogError("Failed to update OrderId {OrderId}", orderId);
                throw new DataRegistrationException("Updation of the order failed");
            }
            _logger.LogInformation("OrderId {OrderId} marked as Cancelled. Status {OldStatus}->{NewStatus}", updatedOrder.OrderId, previousStatusId, updatedOrder.OrderStatusId);

            var orderLog = new LogChanges
            {
                TableName = nameof(Order),
                RecordId = updatedOrder.OrderId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"OrderId={orderId}, OrderStatusId={previousStatusId}",
                NewValue = $"OrderId={updatedOrder.OrderId}, OrderStatusId={updatedOrder.OrderStatusId}",
                ChangedAt = DateTime.Now,
                UserId = order.UserId
            };
            var createdLog = await _logChanges.Create(orderLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", orderLog.TableName, orderLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", orderLog.TableName, orderLog.RecordId);
        }
        else
        {
            _logger.LogInformation("OrderId {OrderId} still has {Count} non-cancelled items. Not marking as Cancelled", orderId, orderItems.Count);
        }
    }

    private async Task CreateRefund(int orderItemId, int refundtypeId, int cancelId, decimal refundAmount, int userId)
    {
        _logger.LogInformation("Creating refund for OrderItemId {OrderItemId}, CancelId {CancelId}, RefundAmount {RefundAmount}", orderItemId, cancelId, refundAmount);

        Refund refund = new Refund();
        refund.OrderItemsId = orderItemId;
        refund.RefundTypeId = refundtypeId;
        refund.ActualRefundAmount = refundAmount;
        refund.RequestedDate = DateTime.Now;
        refund.RefundStatusId = (int)RefundStatusEnum.Processed;

        var createdRefund = await _refundRepsository.Create(refund);
        if (createdRefund == null)
        {
            _logger.LogError("Failed to create Refund for OrderItemId {OrderItemId}", orderItemId);
            throw new DataRegistrationException("Refund creation failed");
        }
        _logger.LogInformation("Refund {RefundId} created for OrderItemId {OrderItemId}", createdRefund.RefundId, orderItemId);

        var refundLog = new LogChanges
        {
            TableName = nameof(Refund),
            RecordId = createdRefund.RefundId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"RefundId={createdRefund.RefundId}, OrderItemsId={createdRefund.OrderItemsId}, ActualRefundAmount={createdRefund.ActualRefundAmount}, RefundStatusId={createdRefund.RefundStatusId}",
            ChangedAt = DateTime.Now,
            UserId = userId
        };
        var createdRefundLog = await _logChanges.Create(refundLog);
        if (createdRefundLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", refundLog.TableName, refundLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", refundLog.TableName, refundLog.RecordId);

        CancelRefund cancelRefund = new CancelRefund();
        cancelRefund.CancelId = cancelId;
        cancelRefund.RefundId = createdRefund.RefundId;

        var createdCancelRefund = await _cancelRefundRepsository.Create(cancelRefund);
        if (createdCancelRefund == null)
        {
            _logger.LogError("Failed to create CancelRefund link for CancelId {CancelId}, RefundId {RefundId}", cancelId, createdRefund.RefundId);
            throw new DataRegistrationException("Cancel-Refund link creation failed");
        }
        _logger.LogInformation("CancelRefund link created for CancelId {CancelId}, RefundId {RefundId}", cancelId, createdRefund.RefundId);

        var cancelRefundLog = new LogChanges
        {
            TableName = nameof(CancelRefund),
            RecordId = createdCancelRefund.CancelRefundId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"CancelRefundId={createdCancelRefund.CancelRefundId}, CancelId={createdCancelRefund.CancelId}, RefundId={createdCancelRefund.RefundId}",
            ChangedAt = DateTime.Now,
            UserId = userId
        };
        var createdCancelRefundLog = await _logChanges.Create(cancelRefundLog);
        if (createdCancelRefundLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", cancelRefundLog.TableName, cancelRefundLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", cancelRefundLog.TableName, cancelRefundLog.RecordId);

        // Credit the refunded amount to the customer's wallet now that the refund is Processed.
        var user = await _userRepsository.Get(userId);
        if (user == null)
        {
            _logger.LogWarning("UserId {UserId} not found while crediting wallet for RefundId {RefundId}", userId, createdRefund.RefundId);
            throw new DataNotFoundException("User Not Found");
        }

        decimal previousWalletCost = user.WalletCost;
        user.WalletCost = user.WalletCost + refundAmount;

        var updatedUser = await _userRepsository.Update(user.UserId, user);
        if (updatedUser == null)
        {
            _logger.LogError("Failed to credit wallet for UserId {UserId}, RefundId {RefundId}", user.UserId, createdRefund.RefundId);
            throw new DataRegistrationException("Wallet credit failed");
        }
        _logger.LogInformation("Wallet credited for UserId {UserId}. WalletCost {OldWalletCost} -> {NewWalletCost} for RefundId {RefundId}",
            updatedUser.UserId, previousWalletCost, updatedUser.WalletCost, createdRefund.RefundId);

        var walletLog = new LogChanges
        {
            TableName = nameof(User),
            RecordId = updatedUser.UserId,
            Actions = (int)AuditAction.Updated,
            OldValue = $"UserId={user.UserId}, WalletCost={previousWalletCost}",
            NewValue = $"UserId={updatedUser.UserId}, WalletCost={updatedUser.WalletCost}",
            UserId = updatedUser.UserId,
            ChangedAt = DateTime.Now
        };

        var createdWalletLog = await _logChanges.Create(walletLog);
        if (createdWalletLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", walletLog.TableName, walletLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }

        var updatedRefund = await _refundRepsository.Get(createdRefund.RefundId);
        updatedRefund!.RefundStatusId = (int)RefundStatusEnum.Completed;
        updatedRefund.ProcessedDate = DateTime.Now;
        await _refundRepsository.Update(updatedRefund.RefundId, updatedRefund);

        var cancel = await _cancelRepsository.Get(cancelId);
        cancel.CancelStatusId = (int)CancelStatusEnum.Refunded;
        await _cancelRepsository.Update(cancelId, cancel);
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", walletLog.TableName, walletLog.RecordId);
    }
}