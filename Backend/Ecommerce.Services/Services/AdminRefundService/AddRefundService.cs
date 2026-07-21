using AutoMapper;
using Ecommerce.Data;
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
    private readonly INotificationService _notificationService;
    private readonly EcommerceContext _ecommerceContext;

    public AdminRefundService(
        IUserRepsository userRepsository,
        ILogChanges logChanges,
        ILogger<AdminRefundService> logger,
        IReturnRepsository returnRepsository,
        IReturnRefundRepsository returnRefundRepsository,
        IPaymentService paymentService,
        IMapper mapper,
        IRefundRepsository refundRepsository,
        IOrderItemRepsository orderItemRepsository,
        INotificationService notificationService,
        EcommerceContext ecommerceContext)
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
        _notificationService = notificationService;
        _ecommerceContext = ecommerceContext;
    }

    public async Task<ResponseAddRefundDTO> CreateReturnRefund(RequestAddReturnRefundDTO requestAddReturnRefundDTO)
    {
        var isNested = _ecommerceContext.Database.CurrentTransaction != null;
        var transaction = isNested
            ? null
            : await _ecommerceContext.Database.BeginTransactionAsync();

        try
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

            var orderItem = await _orderItemRepsository.GetOrderItemByOrderItemId(returnItem.OrderItemId);
            if (orderItem == null)
            {
                _logger.LogWarning("OrderItemId {OrderItemId} not found for ReturnId {ReturnId}", returnItem.OrderItemId, returnItem.ReturnId);
                throw new DataNotFoundException("Order Item Not Found");
            }
            if (requestAddReturnRefundDTO.RefundAmount > returnItem.DamageCost)
            {
                _logger.LogWarning("RefundAmount {RefundAmount} exceeds DamageCost {DamageCost} for ReturnId {ReturnId}",
                    requestAddReturnRefundDTO.RefundAmount, returnItem.DamageCost, returnItem.ReturnId);
                throw new DataApprovalStatusException("Refund amount cannot exceed the recorded damage cost");
            }
            int customerUserId = orderItem.Order!.UserId;

            decimal refundAmount = orderItem.Quantity * orderItem.UnitPrice
                - orderItem.Discount
                - requestAddReturnRefundDTO.RefundAmount - (returnItem.ConvenienceFee ?? 0m);

            decimal orderItemAmount = orderItem.Quantity * orderItem.UnitPrice - orderItem.Discount;

            if (refundAmount > orderItemAmount)
            {
                throw new DataApprovalStatusException("Refund Cost cannot be greater than order item cost");
            }

            if (refundAmount < 0)
            {
                throw new DataApprovalStatusException("Refund amount cannot be negative. Damage cost/deductions exceed the refundable amount.");
            }

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
                UserId = customerUserId,
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
                DamageCost = requestAddReturnRefundDTO.RefundAmount,
                DeductionReason = requestAddReturnRefundDTO.Remarks,
                ReturnId = returnItem.ReturnId
            };

            int previousReturnStatusId = returnItem.ReturnStatusId;
            returnItem.ReturnStatusId = (int)ReturnStatusEnum.RefundProcessed;

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
                ChangedAt = DateTime.Now,
                UserId = customerUserId
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
                UserId = customerUserId,
                ChangedAt = DateTime.Now
            };
            var createdReturnRefundLog = await _logChanges.Create(returnRefundLog);
            if (createdReturnRefundLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", returnRefundLog.TableName, returnRefundLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", returnRefundLog.TableName, returnRefundLog.RecordId);

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

            await _notificationService.SendToUser(
                customerUserId,
                "Refund Processed",
                $"Your refund of {refundAmount:C} for your return has been processed and credited to your wallet.",
                notificationTypeId: (int)NotificationTypeEnum.ReturnRefunded,
                referenceType: "Refund",
                referenceId: createdRefund.RefundId);
            _logger.LogInformation("Refund processed notification sent to customer UserId {UserId} for RefundId {RefundId}", customerUserId, createdRefund.RefundId);

            var updateRefund = await _refundRepsository.Get(createdRefund.RefundId);
            updateRefund.ProcessedDate = DateTime.Now;
            updateRefund.RefundStatusId = (int)RefundStatusEnum.Completed;
            await _refundRepsository.Update(updateRefund.RefundId, updateRefund);

            returnItem.ReviewRemarks = requestAddReturnRefundDTO.Remarks;
            await _returnRepsository.Update(returnItem.ReturnId,returnItem);

            if (transaction != null) await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for ReturnId {ReturnId}, RefundId {RefundId}", returnItem.ReturnId, createdRefund.RefundId);

            return _mapper.Map<ResponseAddRefundDTO>(createdReturnRefund);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while creating return refund for ReturnId {ReturnId}", requestAddReturnRefundDTO.ReturnId);
            if (transaction != null) await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for ReturnId {ReturnId}", requestAddReturnRefundDTO.ReturnId);
            throw;
        }
        finally
        {
            transaction?.Dispose();
        }
    }
}