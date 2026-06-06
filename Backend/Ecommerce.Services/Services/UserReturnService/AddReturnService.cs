using System.Data.Common;
using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class UserReturnService : IUserReturnService
{
    private readonly IReturnRepsository _returnRepsository;
    private readonly IOrderValidation _orderValidation;
    private readonly IShipmentValidation _shipmentValidation;
    private readonly IMapper _mapper;
    public UserReturnService(IReturnRepsository returnRepsository, IOrderValidation orderValidation, IMapper mapper, IShipmentValidation shipmentValidation)
    {
        _returnRepsository = returnRepsository;
        _orderValidation = orderValidation;
        _shipmentValidation = shipmentValidation;
        _mapper = mapper;
    }
    public async Task<ResponseAddReturnDTO> AddReturn(RequestAddReturnDTO requestAddReturnDTO)
    {
        var orderItem = await _orderValidation.ValidateOrderItem(requestAddReturnDTO.OrderItemId);
        var shipment = await _shipmentValidation.ValidateGetShipmentByOrderItemId(orderItem.OrderItemsId);
        if (shipment.DeliveryDate == null || orderItem.OrderItemStatusId != 4)
        {
            throw new DataApprovalStatusException("Order Still Not Delivered Yet");
        }
        if (DateTime.Now < shipment.DeliveryDate.Value.AddDays(7))
        {
            throw new DataApprovalStatusException("Order Item Request Exceeded The date Limit");
        }
        var returnOrder = _mapper.Map<Return>(requestAddReturnDTO);
        returnOrder.RequestedDate = DateTime.Now;
        await _returnRepsository.Create(returnOrder);
        return _mapper.Map<ResponseAddReturnDTO>(returnOrder);
    }
}