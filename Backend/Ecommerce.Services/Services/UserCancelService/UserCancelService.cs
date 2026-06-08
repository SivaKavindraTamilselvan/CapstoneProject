using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class UserCancelService : IUserCancelService
{
    private readonly IOrderRepsository _orderRepsository;
    private readonly IInventoryValidation _inventoryValidation;
    private readonly IOrderValidation _orderValidation;
    private readonly ICancelRepsository _cancelRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IRefundRepsository _refundRepsository;
    private readonly ICancelRefundRepsository _cancelRefundRepsository;
    private readonly IMapper _mapper;
    public UserCancelService(IOrderItemRepsository orderItemRepsository,IRefundRepsository refundRepsository,ICancelRefundRepsository cancelRefundRepsository,IOrderRepsository orderRepsository,IInventoryValidation inventoryValidation,ICancelRepsository cancelRepsository,IOrderValidation orderValidation,IMapper mapper)
    {
        _orderItemRepsository = orderItemRepsository;
        _refundRepsository = refundRepsository;
        _orderRepsository = orderRepsository;
        _cancelRefundRepsository=cancelRefundRepsository;
        _inventoryValidation = inventoryValidation;
        _cancelRepsository = cancelRepsository;
        _orderValidation = orderValidation;
        _mapper = mapper;
    }
    public async Task<ResponseCancelDTO> RequestCancel(RequestCancelDTO requestCancelDTO)
    {
        var order = await _orderValidation.ValidateOrderItem(requestCancelDTO.OrderItemId);
        if(requestCancelDTO.CancelQuantity>order.Quantity)
        {
            throw new DataApprovalStatusException("Quantity Higher Than the Order Quantity Is Not Possible");
        }
        if(order.OrderItemStatusId !=1 && order.OrderItemStatusId!=2)
        {
            throw new DataApprovalStatusException("Order Already Shipped. Cannot be cancelled");
        }
        var orderItemAmount = order.Quantity * order.UnitPrice - order.Discount;
        var ConvenienceFee = orderItemAmount * 0.05m;
        var refundAmount = orderItemAmount - ConvenienceFee;
        var cancel = _mapper.Map<Cancel>(requestCancelDTO);
        cancel.CancelledDate = DateTime.Now;
        cancel.CancelStatusId = 2;
        cancel.ConvenienceFee = ConvenienceFee;
        await _cancelRepsository.Create(cancel);
        await UpdateOrderItemStatus(order.OrderItemsId,order.Quantity);
        await UpdateOrder(order.OrderId);
        await UpdateInventory(order.InventoryId,order.Quantity);
        await CreateRefund(order.OrderItemsId,1,cancel.CancelId,refundAmount);
        return _mapper.Map<ResponseCancelDTO>(cancel);
    }
    private async Task UpdateInventory(int inventoryId,int Quantity)
    {
        var inventory = await _inventoryValidation.ValidateInventory(inventoryId);
        inventory.AvailableQuantity = inventory.AvailableQuantity + Quantity;
        inventory.ReservedQuantity = inventory.ReservedQuantity - Quantity;
        inventory.UpdatedAt = DateTime.Now;
    }
    private async Task UpdateOrderItemStatus(int orderitemId,int Quantity)
    {
        var order = await _orderValidation.ValidateOrderItem(orderitemId);
        if(order.Quantity == Quantity)
        {
            order.OrderItemStatusId = 7;
        }
        else
        {
            order.Quantity = order.Quantity - Quantity;
        }
    }
    private async Task UpdateOrder(int orderId)
    {
        var orderItems = await _orderItemRepsository.GetCancelledOrderItemsByOrderId(orderId);
        var order = await _orderValidation.ValidateOrder(orderId);
        if(orderItems.Count==0)
        {
            order.OrderStatusId = 4;
            order.UpdatedAt = DateTime.Now;
            await _orderRepsository.Update(orderId,order);
        }
    }
    private async Task CreateRefund(int orderItemId,int refundtypeId,int cancelId,decimal refundAmount)
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
