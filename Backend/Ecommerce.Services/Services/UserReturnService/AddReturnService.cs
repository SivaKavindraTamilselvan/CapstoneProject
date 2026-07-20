using System.Data.Common;
using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserReturnService : IUserReturnService
{
    public async Task<ResponseAddReturnDTO> AddReturn(RequestAddReturnDTO requestAddReturnDTO)
    {
        _logger.LogInformation("Processing return request for OrderItemId: {OrderItemId}, ReturnQuantity: {ReturnQuantity}", requestAddReturnDTO.OrderItemId, requestAddReturnDTO.ReturnQuantity);

        var orderItem = await _orderValidation.ValidateOrderItem(requestAddReturnDTO.OrderItemId);

        if (!orderItem.ProductVariant!.IsReturn)
        {
            _logger.LogWarning("Return not allowed for OrderItemId: {OrderItemId}", orderItem.OrderItemsId);
            throw new DataApprovalStatusException("Order Is Not Available For Return");
        }

        if (requestAddReturnDTO.ReturnQuantity > orderItem.Quantity)
        {
            _logger.LogWarning("Invalid return quantity for OrderItemId: {OrderItemId}. Requested: {RequestedQuantity}, Ordered: {OrderedQuantity}", orderItem.OrderItemsId, requestAddReturnDTO.ReturnQuantity, orderItem.Quantity);
            throw new DataApprovalStatusException("Return for quantity Higher Than the Order Quantity Is Not Possible");
        }

        var shipment = await _shipmentValidation.ValidateGetShipmentByOrderItemId(orderItem.OrderItemsId);

        if (shipment.DeliveryDate == null || orderItem.OrderItemStatusId != 4)
        {
            _logger.LogWarning("Return rejected because order is not delivered. OrderItemId: {OrderItemId}", orderItem.OrderItemsId);
            throw new DataApprovalStatusException("Order Still Not Delivered Yet");
        }

        if (DateTime.Now > shipment.DeliveryDate.Value.AddDays(7))
        {
            _logger.LogWarning("Return window expired for OrderItemId: {OrderItemId}", orderItem.OrderItemsId);
            throw new DataApprovalStatusException("Order Item Request Exceeded The date Limit");
        }

        var returns = await _returnRepsository.GetTheReturnProductByOrderItemId(requestAddReturnDTO.OrderItemId);

        if (returns != null)
        {
            _logger.LogWarning("Return already exists for OrderItemId: {OrderItemId}", requestAddReturnDTO.OrderItemId);
            throw new DataApprovalStatusException("Return Already Requested");
        }

        int returnOrderItemId;

        if (orderItem.Quantity == requestAddReturnDTO.ReturnQuantity)
        {
            orderItem.OrderItemStatusId = (int)OrderItemStatusEnum.Return_Requested;
            await _orderItemRepsository.Update(orderItem.OrderItemsId, orderItem);

            _logger.LogInformation("Marked OrderItemId: {OrderItemId} as return requested.", orderItem.OrderItemsId);

            returnOrderItemId = orderItem.OrderItemsId;
        }
        else
        {
            var originalDiscount = orderItem.Discount;
            var originalQuantity = orderItem.Quantity;

            orderItem.Quantity -= requestAddReturnDTO.ReturnQuantity;
            orderItem.Discount = (originalDiscount / originalQuantity) * orderItem.Quantity;

            await _orderItemRepsository.Update(orderItem.OrderItemsId, orderItem);

            _logger.LogInformation("Updated original OrderItemId: {OrderItemId} after partial return. RemainingQuantity: {RemainingQuantity}", orderItem.OrderItemsId, orderItem.Quantity);

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

            _logger.LogInformation("Created return OrderItemId: {OrderItemId} for original OrderItemId: {OriginalOrderItemId}", returnItem.OrderItemsId, orderItem.OrderItemsId);

            returnOrderItemId = returnItem.OrderItemsId;
        }

        var returnOrder = _mapper.Map<Return>(requestAddReturnDTO);
        returnOrder.RequestedDate = DateTime.Now;
        returnOrder.OrderItemId = returnOrderItemId;
        returnOrder.ConvenienceFee = orderItem.UnitPrice * requestAddReturnDTO.ReturnQuantity * 0.15m;

        await _returnRepsository.Create(returnOrder);

        _logger.LogInformation("Return request created successfully. ReturnId: {ReturnId}, OrderItemId: {OrderItemId}", returnOrder.ReturnId, returnOrder.OrderItemId);

        return _mapper.Map<ResponseAddReturnDTO>(returnOrder);
    }
}