using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserOrderService : IUserOrderService
{
    public async Task OnPaymentVerified(int orderId)
    {
        _logger.LogInformation("Processing payment verification for OrderId {OrderId}", orderId);

        var order = await _orderRepsository.Get(orderId);
        if (order == null)
        {
            _logger.LogWarning("OrderId {OrderId} not found while processing payment verification", orderId);
            throw new DataNotFoundException("Order not found");
        }

        await _orderService.ConfirmOrderStatus(orderId, true);
        _logger.LogInformation("OrderId {OrderId} status confirmed as paid", orderId);

        await _userCartService.DeleteAllCart(order.UserId);
        _logger.LogInformation("Cart cleared for UserId {UserId} after successful payment for OrderId {OrderId}", order.UserId, orderId);

        var shipment = await _shipmentRepsository.GetShipmentByOrderId(orderId);
        _logger.LogInformation("Found {ShipmentCount} shipments for OrderId {OrderId} to update after payment verification", shipment.Count, orderId);

        foreach (var items in shipment)
        {
            int previousStatusId = items.ShipmentStatusId;
            items.ShipmentStatusId = (int)ShipmentStatusEnum.Payment_Success;
            await _shipmentRepsository.Update(items.ShipmentId, items);
            _logger.LogInformation("ShipmentId {ShipmentId} status changed from {OldStatus} to {NewStatus} for OrderId {OrderId}", items.ShipmentId, previousStatusId, items.ShipmentStatusId, orderId);
        }

        // notification for order placed sent to user

        var productAdminUserIds = await _adminUserRepsository.GetOrderAdminUserIds();

        if (productAdminUserIds.Count == 0)
        {
            _logger.LogWarning("No order admin users found to notify");
        }

        foreach (var adminUserId in productAdminUserIds)
        {
            await _notificationService.SendToUser(
                adminUserId,
                 "Order Placed Successfully",
                $"Order #{order.OrderId} has been placed successfully.",
                notificationTypeId: (int)NotificationTypeEnum.OrderPlaced,
                referenceType: "Order",
                referenceId: order.OrderId);
            _logger.LogInformation("Order placement notification sent to UserId {UserId} for OrderId {OrderId}", adminUserId, order.OrderId);
        }

        await _notificationService.SendToUser(
            order.UserId,
            "Order Placed Successfully",
            $"Your order #{order.OrderId} has been placed successfully.",
            notificationTypeId: (int)NotificationTypeEnum.OrderPlaced,
            referenceType: "Order",
            referenceId: order.OrderId);
        _logger.LogInformation("Order placement notification sent to UserId {UserId} for OrderId {OrderId}", order.UserId, order.OrderId);

        var orderItem = await _orderItemRepsository.GetOrderItemByOrderId(order.OrderId);

        foreach (var orderItems in orderItem)
        {
            var orderVendors = await _vendorUserRepsository.GetOrderVendorUserByVendorId(orderItems.ProductVariant.Product.VendorId);
            foreach (var vendor in orderVendors)
            {
                await _notificationService.SendToUser(
                vendor.VendorUserId,
                "Order Placed Successfully",
                $"Your order #{order.OrderId} has been placed successfully.",
                notificationTypeId: (int)NotificationTypeEnum.OrderPlaced,
                referenceType: "Order",
                referenceId: order.OrderId);
            }
        }

        _logger.LogInformation("Payment verification processing completed for OrderId {OrderId}", orderId);
    }

    public async Task OnPaymentFailed(int orderId)
    {
        _logger.LogInformation("Processing payment failure for OrderId {OrderId}", orderId);

        var order = await _orderRepsository.Get(orderId);
        if (order == null)
        {
            _logger.LogWarning("OrderId {OrderId} not found while processing payment failure", orderId);
            throw new DataNotFoundException("Order not found");
        }

        await _orderService.ConfirmOrderStatus(orderId, false);
        _logger.LogInformation("OrderId {OrderId} status confirmed as payment failed", orderId);

        var shipment = await _shipmentRepsository.GetShipmentByOrderId(orderId);
        _logger.LogInformation("Found {ShipmentCount} shipments for OrderId {OrderId} to update after payment failure", shipment.Count, orderId);

        foreach (var items in shipment)
        {
            int previousStatusId = items.ShipmentStatusId;
            items.ShipmentStatusId = (int)ShipmentStatusEnum.Payment_Failed;
            await _shipmentRepsository.Update(items.ShipmentId, items);
            _logger.LogInformation("ShipmentId {ShipmentId} status changed from {OldStatus} to {NewStatus} for OrderId {OrderId}", items.ShipmentId, previousStatusId, items.ShipmentStatusId, orderId);
        }

        _logger.LogInformation("Payment failure processing completed for OrderId {OrderId}", orderId);
    }
}