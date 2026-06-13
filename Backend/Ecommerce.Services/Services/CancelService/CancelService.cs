using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class CancelService : ICancelService
{

    private readonly IOrderRepsository _orderRepsository;
    private readonly IInventoryValidation _inventoryValidation;
    private readonly IOrderValidation _orderValidation;
    private readonly ICancelRepsository _cancelRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IRefundRepsository _refundRepsository;
    private readonly ICancelRefundRepsository _cancelRefundRepsository;
    private readonly IMapper _mapper;
    public CancelService(IOrderItemRepsository orderItemRepsository, IRefundRepsository refundRepsository, ICancelRefundRepsository cancelRefundRepsository, IOrderRepsository orderRepsository, IInventoryValidation inventoryValidation, ICancelRepsository cancelRepsository, IOrderValidation orderValidation, IMapper mapper)
    {
        _orderItemRepsository = orderItemRepsository;
        _refundRepsository = refundRepsository;
        _orderRepsository = orderRepsository;
        _cancelRefundRepsository = cancelRefundRepsository;
        _inventoryValidation = inventoryValidation;
        _cancelRepsository = cancelRepsository;
        _orderValidation = orderValidation;
        _mapper = mapper;
    }
}