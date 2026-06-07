using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class AdminRefundService : IAdminRefundService
{
    private readonly IRefundRepsository _refundRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;
    public AdminRefundService(IPaymentService paymentService,IMapper mapper, IRefundRepsository refundRepsository, IOrderItemRepsository orderItemRepsository)
    {
        _mapper = mapper;
        _refundRepsository = refundRepsository;
        _paymentService = paymentService;
        _orderItemRepsository = orderItemRepsository;
    }
    public async Task<ResponseAddRefundDTO> CreateRefund(RequestAddRefundDTO requestAddRefundDTO)
    {
        var order = await _orderItemRepsository.Get(requestAddRefundDTO.OrderItemsId);
        decimal orderItemCost = (order.Quantity * order.UnitPrice) - order.Discount;
        var refund = _mapper.Map<Refund>(requestAddRefundDTO);
        refund.ActualRefundAmount = orderItemCost - requestAddRefundDTO.RefundAmount;
        refund.ProcessedDate = DateTime.Now;
        await _refundRepsository.Create(refund);
        return _mapper.Map<ResponseAddRefundDTO>(refund);
    }
    public async Task<ResponseUpdateRefundDTO> ReviewRefund(RequestUpdateRefundDTO requestUpdateRefundDTO)
    {
        var refund = await _refundRepsository.Get(requestUpdateRefundDTO.RefundId);
        refund.RefundStatusId = requestUpdateRefundDTO.RefundStatusId;
        refund.ActualRefundAmount = requestUpdateRefundDTO.RefundAmount;
        if (requestUpdateRefundDTO.RefundStatusId == 2) // Approved / Initiated
        {
            await _paymentService.ProcessRazorpayRefund(
                refund.RefundId,
                requestUpdateRefundDTO.RefundAmount
            );

            refund.RefundStatusId = 3; // Processed
            refund.ProcessedDate = DateTime.Now;
        }


        await _refundRepsository.Update(refund.RefundId, refund);
        return _mapper.Map<ResponseUpdateRefundDTO>(refund);
    }
}