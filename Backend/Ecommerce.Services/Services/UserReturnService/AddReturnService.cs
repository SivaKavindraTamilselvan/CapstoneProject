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
        var shipment = await _shipmentValidation.ValidateGetShipmentByOrderItemId(orderItem.OrderItemsId);
        if (shipment.DeliveryDate == null || orderItem.OrderItemStatusId != 4)
        {
            throw new DataApprovalStatusException("Order Still Not Delivered Yet");
        }
        if (DateTime.Now > shipment.DeliveryDate.Value.AddDays(7))
        {
            throw new DataApprovalStatusException("Order Item Request Exceeded The date Limit");
        }
        var returnOrder = _mapper.Map<Return>(requestAddReturnDTO);
        returnOrder.RequestedDate = DateTime.Now;
        await _returnRepsository.Create(returnOrder);
        return _mapper.Map<ResponseAddReturnDTO>(returnOrder);
    }
}