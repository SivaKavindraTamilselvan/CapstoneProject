using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class CancelService : ICancelService
{
    public async Task<ResponseCancelDTO> RequestCancel(RequestCancelDTO requestCancelDTO)
    {
        var order = await _orderValidation.ValidateOrderItem(requestCancelDTO.OrderItemId);
        if (requestCancelDTO.CancelQuantity > order.Quantity)
        {
            throw new DataApprovalStatusException("Quantity Higher Than the Order Quantity Is Not Possible");
        }
        if (order.OrderItemStatusId != 1 && order.OrderItemStatusId != 2)
        {
            throw new DataApprovalStatusException("Order Already Shipped. Cannot be cancelled");
        }
        var orderItemAmount = order.Quantity * order.UnitPrice - order.Discount;
        var ConvenienceFee = orderItemAmount * 0.05m;
        var refundAmount = orderItemAmount - ConvenienceFee;

        var cancelledOrderItemId = await UpdateOrderItemStatus(order.OrderItemsId, requestCancelDTO.CancelQuantity);

        var cancel = _mapper.Map<Cancel>(requestCancelDTO);
        cancel.CancelledDate = DateTime.Now;
        cancel.OrderItemId = cancelledOrderItemId;
        cancel.CancelStatusId = 2;
        cancel.ConvenienceFee = ConvenienceFee;
        await _cancelRepsository.Create(cancel);

        await UpdateOrder(order.OrderId);
        await UpdateInventory(order.InventoryId, requestCancelDTO.CancelQuantity);
        await CreateRefund(order.OrderItemsId, 1, cancel.CancelId, refundAmount);
        return _mapper.Map<ResponseCancelDTO>(cancel);
    }
    private async Task UpdateInventory(int inventoryId, int Quantity)
    {
        var inventory = await _inventoryValidation.ValidateInventory(inventoryId);
        inventory.AvailableQuantity = inventory.AvailableQuantity + Quantity;
        inventory.ReservedQuantity = inventory.ReservedQuantity - Quantity;
        inventory.UpdatedAt = DateTime.Now;
    }
    private async Task<int> UpdateOrderItemStatus(int orderItemId, int cancelQuantity)
    {
        var orderItem = await _orderValidation.ValidateOrderItem(orderItemId);

        if (orderItem.Quantity == cancelQuantity)
        {
            orderItem.OrderItemStatusId = 7;
            await _orderItemRepsository.Update(orderItem.OrderItemsId, orderItem);

            return orderItem.OrderItemsId;
        }
        orderItem.Quantity -= cancelQuantity;
        await _orderItemRepsository.Update(orderItem.OrderItemsId, orderItem);

        var cancelledItem = new OrderItems
        {
            OrderId = orderItem.OrderId,
            ProductVariantId = orderItem.ProductVariantId,
            InventoryId = orderItem.InventoryId,
            Quantity = cancelQuantity,
            UnitPrice = orderItem.UnitPrice,
            Discount = (orderItem.Discount / (orderItem.Quantity + cancelQuantity)) * cancelQuantity,
            OrderItemStatusId = 7,

        };

        await _orderItemRepsository.Create(cancelledItem);

        return cancelledItem.OrderItemsId;
    }
    private async Task UpdateOrder(int orderId)
    {
        var orderItems = await _orderItemRepsository.GetCancelledOrderItemsByOrderId(orderId);
        var order = await _orderValidation.ValidateOrder(orderId);
        if (orderItems.Count == 0)
        {
            order.OrderStatusId = 4;
            order.UpdatedAt = DateTime.Now;
            await _orderRepsository.Update(orderId, order);
        }
    }
    private async Task CreateRefund(int orderItemId, int refundtypeId, int cancelId, decimal refundAmount)
    {
        Refund refund = new Refund();
        refund.OrderItemsId = orderItemId;
        refund.RefundTypeId = refundtypeId;
        refund.ActualRefundAmount = refundAmount;
        refund.RequestedDate = DateTime.Now;
        refund.ProcessedDate = DateTime.Now;
        refund.RefundStatusId = 4;
        await _refundRepsository.Create(refund);

        CancelRefund cancelRefund = new CancelRefund();
        cancelRefund.CancelId = cancelId;
        cancelRefund.RefundId = refund.RefundId;
        await _cancelRefundRepsository.Create(cancelRefund);
    }
}