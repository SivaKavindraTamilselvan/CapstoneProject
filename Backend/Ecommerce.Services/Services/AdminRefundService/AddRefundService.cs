using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class AdminRefundService : IAdminRefundService
{
    private readonly IUserRepsository _userRepsository;
    private readonly ILogChanges _logChanges;
    private readonly ILogger<AdminRefundService> _logger;
    private readonly IReturnRepsository _returnRepsository;
    private readonly IRefundRepsository _refundRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IPaymentService _paymentService;
    private readonly IReturnRefundRepsository _returnRefundRepsository;
    private readonly IMapper _mapper;
    public AdminRefundService(IUserRepsository userRepsository,ILogChanges logChanges,ILogger<AdminRefundService> logger,IReturnRepsository returnRepsository, IReturnRefundRepsository returnRefundRepsository, IPaymentService paymentService, IMapper mapper, IRefundRepsository refundRepsository, IOrderItemRepsository orderItemRepsository)
    {
        _userRepsository = userRepsository;
        _logChanges = logChanges;
        _logger = logger;
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
    public async Task<ResponseAddRefundDTO> CreateReturnRefund(RequestAddReturnRefundDTO requestAddReturnRefundDTO)
    {
        _logger.LogInformation("Creating return refund for ReturnId {ReturnId}", requestAddReturnRefundDTO.ReturnId);

        var returnItem = await _returnRepsository.Get(requestAddReturnRefundDTO.ReturnId);
        if (returnItem == null)
        {
            _logger.LogWarning("ReturnId {ReturnId} not found", requestAddReturnRefundDTO.ReturnId);
            throw new DataNotFoundException("Return Not Found");
        }

        var existingReturnRefund = await _returnRefundRepsository.Get(returnItem.ReturnId);
        if (existingReturnRefund != null)
        {
            _logger.LogWarning("Refund already exists for ReturnId {ReturnId}", returnItem.ReturnId);
            throw new DataAlreadyRegisteredException("Refund already created for this return");
        }

        var orderItem = await _orderItemRepsository.Get(returnItem.OrderItemId);
        if (orderItem == null)
        {
            _logger.LogWarning("OrderItemId {OrderItemId} not found for ReturnId {ReturnId}", returnItem.OrderItemId, returnItem.ReturnId);
            throw new DataNotFoundException("Order Item Not Found");
        }

        decimal refundAmount = orderItem.Quantity * orderItem.UnitPrice
            - orderItem.Discount
            - requestAddReturnRefundDTO.RefundAmount;

        var refund = new Refund
        {
            RefundTypeId = 2,
            OrderItemsId = returnItem.OrderItemId,
            ActualRefundAmount = refundAmount,
            RequestedDate = DateTime.Now,
        };

        var createdRefund = await _refundRepsository.Create(refund);
        if (createdRefund == null)
        {
            _logger.LogError("Failed to create Refund for ReturnId {ReturnId}, OrderItemId {OrderItemId}", returnItem.ReturnId, returnItem.OrderItemId);
            throw new DataRegistrationException("Refund creation failed");
        }
        _logger.LogInformation("Refund {RefundId} created for ReturnId {ReturnId}. ActualRefundAmount {RefundAmount}", createdRefund.RefundId, returnItem.ReturnId, refundAmount);

        var refundLog = new LogChanges
        {
            TableName = nameof(Refund),
            RecordId = createdRefund.RefundId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"RefundId={createdRefund.RefundId}, OrderItemsId={createdRefund.OrderItemsId}, ActualRefundAmount={createdRefund.ActualRefundAmount}",
            ChangedAt = DateTime.Now
        };
        var createdRefundLog = await _logChanges.Create(refundLog);
        if (createdRefundLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", refundLog.TableName, refundLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", refundLog.TableName, refundLog.RecordId);

        var returnRefund = new ReturnRefund
        {
            RefundId = createdRefund.RefundId,
            DamageCost = returnItem.DamageCost ?? 0,
            DeductionReason = requestAddReturnRefundDTO.Remarks,
            ReturnId = returnItem.ReturnId
        };

        int previousReturnStatusId = returnItem.ReturnStatusId;
        returnItem.ReturnStatusId = 8;

        var updatedReturn = await _returnRepsository.Update(returnItem.ReturnId, returnItem);
        if (updatedReturn == null)
        {
            _logger.LogError("Failed to update ReturnId {ReturnId}", returnItem.ReturnId);
            throw new DataRegistrationException("Updation of the return failed");
        }
        _logger.LogInformation("ReturnId {ReturnId} status changed from {OldStatus} to {NewStatus}", updatedReturn.ReturnId, previousReturnStatusId, updatedReturn.ReturnStatusId);

        var returnLog = new LogChanges
        {
            TableName = nameof(Return),
            RecordId = updatedReturn.ReturnId,
            Actions = (int)AuditAction.Updated,
            OldValue = $"ReturnId={returnItem.ReturnId}, ReturnStatusId={previousReturnStatusId}",
            NewValue = $"ReturnId={updatedReturn.ReturnId}, ReturnStatusId={updatedReturn.ReturnStatusId}",
            ChangedAt = DateTime.Now
        };
        var createdReturnLog = await _logChanges.Create(returnLog);
        if (createdReturnLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", returnLog.TableName, returnLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", returnLog.TableName, returnLog.RecordId);

        var createdReturnRefund = await _returnRefundRepsository.Create(returnRefund);
        if (createdReturnRefund == null)
        {
            _logger.LogError("Failed to create ReturnRefund for ReturnId {ReturnId}, RefundId {RefundId}", returnItem.ReturnId, createdRefund.RefundId);
            throw new DataRegistrationException("Return refund creation failed");
        }
        _logger.LogInformation("ReturnRefund {ReturnRefundId} created for ReturnId {ReturnId}, RefundId {RefundId}", createdReturnRefund.ReturnRefundId, returnItem.ReturnId, createdRefund.RefundId);

        var returnRefundLog = new LogChanges
        {
            TableName = nameof(ReturnRefund),
            RecordId = createdReturnRefund.ReturnRefundId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"ReturnRefundId={createdReturnRefund.ReturnRefundId}, ReturnId={createdReturnRefund.ReturnId}, RefundId={createdReturnRefund.RefundId}, DamageCost={createdReturnRefund.DamageCost}",
            ChangedAt = DateTime.Now
        };
        var createdReturnRefundLog = await _logChanges.Create(returnRefundLog);
        if (createdReturnRefundLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", returnRefundLog.TableName, returnRefundLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", returnRefundLog.TableName, returnRefundLog.RecordId);

        // Credit the refunded amount to the customer's wallet now that the return refund is recorded.
        // Resolve the customer via the order item -> order, since this method isn't given userId directly.
        int customerUserId = orderItem.Order.UserId;
        var user = await _userRepsository.Get(customerUserId);
        if (user == null)
        {
            _logger.LogWarning("UserId {UserId} not found while crediting wallet for RefundId {RefundId}", customerUserId, createdRefund.RefundId);
            throw new DataNotFoundException("User Not Found");
        }

        decimal previousWalletCost = user.WalletCost;
        user.WalletCost = user.WalletCost + refundAmount;

        var updatedUser = await _userRepsository.Update(user.UserId, user);
        if (updatedUser == null)
        {
            _logger.LogError("Failed to credit wallet for UserId {UserId}, RefundId {RefundId}", user.UserId, createdRefund.RefundId);
            throw new DataRegistrationException("Wallet credit failed");
        }
        _logger.LogInformation("Wallet credited for UserId {UserId}. WalletCost {OldWalletCost} -> {NewWalletCost} for RefundId {RefundId}",
            updatedUser.UserId, previousWalletCost, updatedUser.WalletCost, createdRefund.RefundId);

        var walletLog = new LogChanges
        {
            TableName = nameof(User),
            RecordId = updatedUser.UserId,
            Actions = (int)AuditAction.Updated,
            OldValue = $"UserId={user.UserId}, WalletCost={previousWalletCost}",
            NewValue = $"UserId={updatedUser.UserId}, WalletCost={updatedUser.WalletCost}",
            UserId = updatedUser.UserId,
            ChangedAt = DateTime.Now
        };

        var createdWalletLog = await _logChanges.Create(walletLog);
        if (createdWalletLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", walletLog.TableName, walletLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", walletLog.TableName, walletLog.RecordId);

        return _mapper.Map<ResponseAddRefundDTO>(createdReturnRefund);
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