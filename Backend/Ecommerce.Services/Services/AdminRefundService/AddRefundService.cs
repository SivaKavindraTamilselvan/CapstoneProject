using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class AdminRefundService : IAdminRefundService
{
    private readonly IReturnRepsository _returnRepsository;
    private readonly IRefundRepsository _refundRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IPaymentService _paymentService;
    private readonly IReturnRefundRepsository _returnRefundRepsository;
    private readonly IMapper _mapper;
    public AdminRefundService(IReturnRepsository returnRepsository, IReturnRefundRepsository returnRefundRepsository, IPaymentService paymentService, IMapper mapper, IRefundRepsository refundRepsository, IOrderItemRepsository orderItemRepsository)
    {
        _returnRepsository = returnRepsository;
        _mapper = mapper;
        _refundRepsository = refundRepsository;
        _paymentService = paymentService;
        _orderItemRepsository = orderItemRepsository;
        _returnRefundRepsository = returnRefundRepsository;
    }
    public async Task<ResponseAddRefundDTO> CreateRefund(RequestAddRefundDTO requestAddRefundDTO)
    {
        var order = await _orderItemRepsository.Get(requestAddRefundDTO.OrderItemsId);
        if (order == null)
        {
            throw new DataNotFoundException("OrderItem Not Found");
        }
        var refund = _mapper.Map<Refund>(requestAddRefundDTO);
        refund.ProcessedDate = DateTime.Now;
        if (requestAddRefundDTO.RefundTypeId == 1)
        {
            refund.ActualRefundAmount = (order.Quantity * order.UnitPrice) - order.Discount;
        }
        await _refundRepsository.Create(refund);
        return _mapper.Map<ResponseAddRefundDTO>(refund);
    }
    public async Task<ResponseAddRefundDTO> CreateReturnRefund(
     RequestAddReturnRefundDTO requestAddReturnRefundDTO)
    {
        var returnItem = await _returnRepsository.Get(requestAddReturnRefundDTO.ReturnId);

        if (returnItem == null)
        {
            throw new DataNotFoundException("Return Not Found");
        }

        var existingReturnRefund =
            await _returnRefundRepsository.Get(returnItem.ReturnId);

        if (existingReturnRefund != null)
        {
            throw new DataAlreadyRegisteredException(
                "Refund already created for this return");
        }

        var orderItem = await _orderItemRepsository.Get(returnItem.OrderItemId);

        if (orderItem == null)
        {
            throw new DataNotFoundException("Order Item Not Found");
        }

        var refund = new Refund
        {
            RefundTypeId = 2,
            OrderItemsId = returnItem.OrderItemId,
            ActualRefundAmount =
                orderItem.Quantity * orderItem.UnitPrice
                - orderItem.Discount
                - requestAddReturnRefundDTO.RefundAmount,
            RequestedDate = DateTime.Now
        };

        var createdRefund = await _refundRepsository.Create(refund);

        var returnRefund = new ReturnRefund
        {
            RefundId = createdRefund!.RefundId,
            DamageCost = returnItem.DamageCost ?? 0,
            DeductionReason = requestAddReturnRefundDTO.Remarks,
            ReturnId = returnItem.ReturnId
        };

        await _returnRefundRepsository.Create(returnRefund);

        return _mapper.Map<ResponseAddRefundDTO>(returnRefund);
    }
    public async Task<ResponseUpdateRefundDTO> ReviewRefund(RequestUpdateRefundDTO requestUpdateRefundDTO)
    {
        var refund = await _refundRepsository.Get(requestUpdateRefundDTO.RefundId);
        if (refund == null)
        {
            throw new DataNotFoundException("Refund Data Not Found");
        }
        // needed to add refund status too cancel and return
        refund.RefundStatusId = 7;
        refund.ProcessedDate = DateTime.Now;
        await _refundRepsository.Update(refund.RefundId, refund);
        return _mapper.Map<ResponseUpdateRefundDTO>(refund);
    }
}