using System.Data.Common;
using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserReturnService : IUserReturnService
{
    public async Task<ResponseAddReturnDTO> AddReturn(RequestAddReturnDTO requestAddReturnDTO)
    {
        var orderItem = await _orderValidation.ValidateOrderItem(requestAddReturnDTO.OrderItemId);

        if (!orderItem.ProductVariant!.IsReturn)
        {
            throw new DataApprovalStatusException("Order Is Not Available For Return");
        }
        if (requestAddReturnDTO.ReturnQuantity > orderItem.Quantity)
        {
            throw new DataApprovalStatusException("Return for quantity Higher Than the Order Quantity Is Not Possible");
        }
        var shipment = await _shipmentValidation.ValidateGetShipmentByOrderItemId(orderItem.OrderItemsId);
        if (shipment.DeliveryDate == null || orderItem.OrderItemStatusId != 4)
        {
            throw new DataApprovalStatusException("Order Still Not Delivered Yet");
        }
        if (DateTime.Now > shipment.DeliveryDate.Value.AddDays(7))
        {
            throw new DataApprovalStatusException("Order Item Request Exceeded The date Limit");
        }
        var returns = await _returnRepsository.GetTheReturnProductByOrderItemId(requestAddReturnDTO.OrderItemId);
        if(returns != null)
        {
            throw new DataApprovalStatusException("Return Already Requested");
        }
       
        int returnOrderItemId;

        if (orderItem.Quantity == requestAddReturnDTO.ReturnQuantity)
        {
            orderItem.OrderItemStatusId = 9;
            await _orderItemRepsository.Update(orderItem.OrderItemsId, orderItem);

            returnOrderItemId = orderItem.OrderItemsId;
        }
        else
        {
            var originalDiscount = orderItem.Discount;
            var originalQuantity = orderItem.Quantity;

            orderItem.Quantity -= requestAddReturnDTO.ReturnQuantity;
            orderItem.Discount = (originalDiscount / originalQuantity) * orderItem.Quantity;
            await _orderItemRepsository.Update(orderItem.OrderItemsId, orderItem);

            var returnItem = new OrderItems
            {
                OrderId = orderItem.OrderId,
                ProductVariantId = orderItem.ProductVariantId,
                InventoryId = orderItem.InventoryId,
                Quantity = requestAddReturnDTO.ReturnQuantity,
                UnitPrice = orderItem.UnitPrice,
                Discount = (originalDiscount / originalQuantity) * requestAddReturnDTO.ReturnQuantity,
                OrderItemStatusId = 9,
            };

            await _orderItemRepsository.Create(returnItem);
            returnOrderItemId = returnItem.OrderItemsId;
        }

        var returnOrder = _mapper.Map<Return>(requestAddReturnDTO);
        returnOrder.RequestedDate = DateTime.Now;
        returnOrder.OrderItemId = returnOrderItemId;
        returnOrder.ConvenienceFee = orderItem.UnitPrice * requestAddReturnDTO.ReturnQuantity * 0.15m;

        await _returnRepsository.Create(returnOrder);

        return _mapper.Map<ResponseAddReturnDTO>(returnOrder);
    }
}