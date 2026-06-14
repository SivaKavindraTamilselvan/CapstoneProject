using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class AdminRefundService : IAdminRefundService
{
    private readonly IRefundRepsository _refundRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IPaymentService _paymentService;
    private readonly IReturnRefundRepsository _returnRefundRepsository;
    private readonly IMapper _mapper;
    public AdminRefundService(IReturnRefundRepsository returnRefundRepsository,IPaymentService paymentService, IMapper mapper, IRefundRepsository refundRepsository, IOrderItemRepsository orderItemRepsository)
    {
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
    public async Task<ResponseAddRefundDTO> CreateReturnRefund(RequestAddReturnRefundDTO requestAddReturnRefundDTO)
    {
        var refund = _mapper.Map<RequestAddRefundDTO>(requestAddReturnRefundDTO);
        var createdRefund = await CreateRefund(refund);
        var ReturnRefund = _mapper.Map<ReturnRefund>(requestAddReturnRefundDTO);
        ReturnRefund.RefundId = createdRefund.RefundId;
        var order = await _orderItemRepsository.Get(requestAddReturnRefundDTO.OrderItemsId);
        if (order == null)
        {
            throw new DataNotFoundException("OrderItem Not Found");
        }
        decimal orderItemCost = (order.Quantity * order.UnitPrice) - order.Discount;
        if(requestAddReturnRefundDTO.DamageCost>orderItemCost)
        {
            throw new DataApprovalStatusException("DamageCost is greater than the original cost");
        }
        await _returnRefundRepsository.Create(ReturnRefund);
        return _mapper.Map<ResponseAddRefundDTO>(ReturnRefund);
    }
    public async Task<ResponseUpdateRefundDTO> ReviewRefund(RequestUpdateRefundDTO requestUpdateRefundDTO)
    {
        var refund = await _refundRepsository.Get(requestUpdateRefundDTO.RefundId);
        if(refund == null)
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