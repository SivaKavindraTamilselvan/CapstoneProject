using System.Data.Common;
using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserReturnService : IUserReturnService
{
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IReturnRepsository _returnRepsository;
    private readonly IOrderValidation _orderValidation;
    private readonly IShipmentValidation _shipmentValidation;
    private readonly IMapper _mapper;
    public UserReturnService(IOrderItemRepsository orderItemRepsository,IReturnRepsository returnRepsository, IOrderValidation orderValidation, IMapper mapper, IShipmentValidation shipmentValidation)
    {
        _orderItemRepsository = orderItemRepsository;
        _returnRepsository = returnRepsository;
        _orderValidation = orderValidation;
        _shipmentValidation = shipmentValidation;
        _mapper = mapper;
    }
}