using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;

public partial class OrderService : IOrderService
{
    //create order 
    // linked with the cart
    public async Task<Order> CreateOrder(RequestCreateOrderDTO requestCreateOrderDTO)
    {
        var order = _mapper.Map<Order>(requestCreateOrderDTO);

        var user = await _userRepsository.Get(requestCreateOrderDTO.UserId);
        if (user == null)
        {
            throw new DataNotFoundException("User not found");
        }

        order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
        order.FinalAmount = order.TotalProductAmount - order.TotalCouponAmount;
        order.FinalAmount = order.FinalAmount * 1.18m + order.TotalShippingAmount;
        order.OrderStatusId = (int)OrderStatusEnum.Pending;
        order.OrderDate = DateTime.Now;

        if (requestCreateOrderDTO.useWallet == true)
        {

            var usedAmount = Math.Min(user.WalletCost, order.FinalAmount);


            order.FinalAmount -= usedAmount;

            var previousWalletCost = user.WalletCost;
            user.WalletCost = user.WalletCost - usedAmount;
            order.TotalWalletAmount = usedAmount;

            var updatedUser = await _userRepsository.Update(user.UserId, user);
            if (updatedUser == null)
            {
                _logger.LogError("Failed to debit wallet for UserId {UserId}", user.UserId);
                throw new DataRegistrationException("Wallet debit failed");
            }
            _logger.LogInformation("Wallet debited for UserId {UserId}. WalletCost {OldWalletCost} -> {NewWalletCost}, Amount used {UsedAmount}",
                user.UserId, previousWalletCost, user.WalletCost, usedAmount);
        }

        await _orderRepsository.Create(order);
        return order;
    }
    //create order 
    // linked with the cart
    public async Task<List<OrderItems>> CreateOrderItems(List<SelectedCartInventory> selectedItems, Order order, Coupons? selectedCoupon)
    {
        _logger.LogInformation("Creating order items for OrderId {OrderId}. SelectedItemCount {Count}", order.OrderId, selectedItems.Count);

        List<OrderItems> orderItemsList = new List<OrderItems>();

        foreach (var selected in selectedItems)
        {
            var cartItem = selected.CartItem;

            OrderItems orderItems = new OrderItems();
            orderItems.ProductVariantId = cartItem.ProductVariantId;
            orderItems.InventoryId = selected.Inventory.InventoryId;
            orderItems.Quantity = cartItem.Qunatity;
            orderItems.OrderId = order.OrderId;
            orderItems.UnitPrice = cartItem.ProductVariant!.Price;
            orderItems.Discount = 0;
            orderItems.OrderItemStatusId = (int)OrderItemStatusEnum.Pending;

            var productVariant = await _productVariantRepsository.Get(cartItem.ProductVariantId);

            if (cartItem.Qunatity > productVariant!.MinimuQuantityPerUser)
            {
                throw new DataRegistrationException(
                    $"You can purchase a maximum of {productVariant.MinimuQuantityPerUser} units of this product."
                );
            }
            if (selectedCoupon != null)
            {
                bool applicable = selectedCoupon.CouponsProducts.Any(p => p.ProductId == cartItem.ProductVariant!.ProductId);
                if (applicable)
                {
                    orderItems.Discount = selectedCoupon.DiscountValue;
                    _logger.LogInformation("CouponId {CouponId} applied to ProductVariantId {ProductVariantId}. Discount {Discount}", selectedCoupon.CouponId, cartItem.ProductVariantId, orderItems.Discount);
                }
            }

            var createdOrderItem = await _orderItemRepsository.Create(orderItems);
            if (createdOrderItem == null)
            {
                _logger.LogError("Failed to create OrderItem for OrderId {OrderId}, ProductVariantId {ProductVariantId}", order.OrderId, cartItem.ProductVariantId);
                throw new DataRegistrationException("Order item creation failed");
            }
            orderItemsList.Add(createdOrderItem);
            _logger.LogInformation("OrderItem {OrderItemsId} created for OrderId {OrderId}, ProductVariantId {ProductVariantId}, Quantity {Quantity}", createdOrderItem.OrderItemsId, order.OrderId, createdOrderItem.ProductVariantId, createdOrderItem.Quantity);

            var orderItemLog = new LogChanges
            {
                TableName = nameof(OrderItems),
                RecordId = createdOrderItem.OrderItemsId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"OrderItemsId={createdOrderItem.OrderItemsId}, OrderId={createdOrderItem.OrderId}, ProductVariantId={createdOrderItem.ProductVariantId}, Quantity={createdOrderItem.Quantity}, UnitPrice={createdOrderItem.UnitPrice}, Discount={createdOrderItem.Discount}",
                ChangedAt = DateTime.Now,
                UserId = order.UserId,
            };

            var createdOrderItemLog = await _logChanges.Create(orderItemLog);
            if (createdOrderItemLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", orderItemLog.TableName, orderItemLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", orderItemLog.TableName, orderItemLog.RecordId);

        }

        if (selectedCoupon != null)
        {
            _logger.LogInformation("Recording coupon usage for OrderId {OrderId}, CouponId {CouponId}", order.OrderId, selectedCoupon.CouponId);
            await CreateCouponUsage(order.OrderId, selectedCoupon.CouponId);
        }

        _logger.LogInformation("CreateOrderItems completed for OrderId {OrderId}. {Count} order items created", order.OrderId, orderItemsList.Count);
        return orderItemsList;
    }

    private async Task CreateCouponUsage(int orderId, int couponId)
    {
        _logger.LogInformation("Creating CouponUsage record for OrderId {OrderId}, CouponId {CouponId}", orderId, couponId);

        CouponUsage couponUsage = new CouponUsage();
        couponUsage.CouponId = couponId;
        couponUsage.OrderId = orderId;

        var order = await _orderRepsository.Get(orderId);
        if (order == null)
        {
            throw new DataNotFoundException("Order Id not found for coupon usage creation");
        }

        var createdCouponUsage = await _couponUsageRepsository.Create(couponUsage);
        if (createdCouponUsage == null)
        {
            _logger.LogError("Failed to create CouponUsage for OrderId {OrderId}, CouponId {CouponId}", orderId, couponId);
            throw new DataRegistrationException("Coupon usage creation failed");
        }
        _logger.LogInformation("CouponUsage {CouponUsageId} created for OrderId {OrderId}, CouponId {CouponId}", createdCouponUsage.CouponUsageId, orderId, couponId);

        var couponUsageLog = new LogChanges
        {
            TableName = nameof(CouponUsage),
            RecordId = createdCouponUsage.CouponUsageId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"CouponUsageId={createdCouponUsage.CouponUsageId}, OrderId={createdCouponUsage.OrderId}, CouponId={createdCouponUsage.CouponId}",
            ChangedAt = DateTime.Now,
            UserId = order.UserId
        };

        var createdLog = await _logChanges.Create(couponUsageLog);
        if (createdLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", couponUsageLog.TableName, couponUsageLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", couponUsageLog.TableName, couponUsageLog.RecordId);
    }

}