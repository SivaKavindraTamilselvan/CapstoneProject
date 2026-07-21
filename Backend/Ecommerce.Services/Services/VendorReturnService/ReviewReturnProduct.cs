using Ecommerce.DTOs;
using Ecommerce.DTOs.Returns;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorReturnService : IVendorReturnService
{
    public async Task<ResponseReviewReturnDTO> ReviewReturnOrderProduct(RequestReviewReturnProductDTO requestReviewReturnProductDTO, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} reviewing ReturnId {ReturnId}", vendorUserId, requestReviewReturnProductDTO.ReturnId);

            var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            var returnItem = await _returnRepsository.Get(requestReviewReturnProductDTO.ReturnId);
            if (returnItem == null)
            {
                _logger.LogWarning("ReturnId {ReturnId} not found for Vendor UserId {VendorUserId}", requestReviewReturnProductDTO.ReturnId, vendorUserId);
                throw new DataNotFoundException("Return Not Found");
            }

            var orderItem = await _orderItemRepsository.GetOrderItemByOrderItemId(returnItem.OrderItemId);
            if (orderItem == null)
            {
                _logger.LogWarning("OrderItemId {OrderItemId} not found for ReturnId {ReturnId}", returnItem.OrderItemId, returnItem.ReturnId);
                throw new DataNotFoundException("Order Item Not Found");
            }

            decimal orderItemAmount = orderItem.Quantity * orderItem.UnitPrice - orderItem.Discount;
            decimal maxAllowedDamageCost = orderItemAmount * 0.5m;

            if (requestReviewReturnProductDTO.DamageCost > maxAllowedDamageCost)
            {
                _logger.LogWarning("DamageCost {DamageCost} exceeds 50% of order item cost {MaxAllowed} for ReturnId {ReturnId}",
                    requestReviewReturnProductDTO.DamageCost, maxAllowedDamageCost, returnItem.ReturnId);
                throw new DataApprovalStatusException("Damage cost cannot exceed 50% of the refundable order item amount");
            }

            int previousReturnStatusId = returnItem.ReturnStatusId;


            returnItem.DamageCost = requestReviewReturnProductDTO.DamageCost;
            returnItem.VendorReview = requestReviewReturnProductDTO.Remarks;
            returnItem.ReviewedByVendorId = vendorUser.VendorUserId;
            returnItem.ReviewedDate = DateTime.Now;
            returnItem.ReturnStatusId = (int)ReturnStatusEnum.DisputeReturn;

            var updatedReturn = await _returnRepsository.Update(returnItem.ReturnId, returnItem);
            if (updatedReturn == null)
            {
                _logger.LogError("Failed to update ReturnId {ReturnId}", returnItem.ReturnId);
                throw new DataRegistrationException("Updation of the return failed");
            }
            _logger.LogInformation("ReturnId {ReturnId} reviewed successfully by Vendor UserId {VendorUserId}. DamageCost: {DamageCost}", updatedReturn.ReturnId, vendorUserId, updatedReturn.DamageCost);

            var returnLog = new LogChanges
            {
                TableName = nameof(Return),
                RecordId = updatedReturn.ReturnId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ReturnId={returnItem.ReturnId}, ReturnStatusId={previousReturnStatusId}",
                NewValue = $"ReturnId={updatedReturn.ReturnId}, ReturnStatusId={updatedReturn.ReturnStatusId}, DamageCost={updatedReturn.DamageCost}",
                UserId = vendorUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(returnLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", returnLog.TableName, returnLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", returnLog.TableName, returnLog.RecordId);

            // Notify the customer that the vendor has disputed the return
            var customerUserId = returnItem.OrderItems?.Order?.UserId;
            if (customerUserId != null && customerUserId != 0)
            {
                await _notificationService.SendToUser(
                    customerUserId.Value,
                    "Return Disputed By Vendor",
                    $"Your return request has been reviewed and marked as disputed. Reason: {returnItem.VendorReview}",
                    notificationTypeId: (int)NotificationTypeEnum.ReturnDisputed,
                    referenceType: "Return",
                    referenceId: updatedReturn.ReturnId);
                _logger.LogInformation("Return dispute notification sent to customer UserId {UserId}", customerUserId);
            }
            else
            {
                _logger.LogWarning("No customer UserId found for ReturnId {ReturnId}. Skipping customer notification", updatedReturn.ReturnId);
            }

            await transaction.CommitAsync();

            return _mapper.Map<ResponseReviewReturnDTO>(updatedReturn);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex,
                "Transaction failed while reviewing ReturnId {ReturnId}, VendorUserId {VendorUserId}",
                requestReviewReturnProductDTO.ReturnId,
                vendorUserId);

            throw;
        }
    }

    public async Task<ResponseReviewReturnDTO> AcceptReturnProduct(int returnId, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} reviewing ReturnId {ReturnId}", vendorUserId, returnId);

            var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            var returnItem = await _returnRepsository.Get(returnId);
            if (returnItem == null)
            {
                _logger.LogWarning("ReturnId {ReturnId} not found for Vendor UserId {VendorUserId}", returnId, vendorUserId);
                throw new DataNotFoundException("Return Not Found");
            }

            int previousReturnStatusId = returnItem.ReturnStatusId;

            returnItem.ReviewedByVendorId = vendorUser.VendorUserId;
            returnItem.ReviewedDate = DateTime.Now;
            returnItem.ReturnStatusId = (int)ReturnStatusEnum.RefundProcessed;

            var updatedReturn = await _returnRepsository.Update(returnItem.ReturnId, returnItem);
            if (updatedReturn == null)
            {
                _logger.LogError("Failed to update ReturnId {ReturnId}", returnItem.ReturnId);
                throw new DataRegistrationException("Updation of the return failed");
            }
            _logger.LogInformation("ReturnId {ReturnId} accepted by Vendor UserId {VendorUserId}", updatedReturn.ReturnId, vendorUserId);

            var returnLog = new LogChanges
            {
                TableName = nameof(Return),
                RecordId = updatedReturn.ReturnId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ReturnId={returnItem.ReturnId}, ReturnStatusId={previousReturnStatusId}",
                NewValue = $"ReturnId={updatedReturn.ReturnId}, ReturnStatusId={updatedReturn.ReturnStatusId}",
                UserId = vendorUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(returnLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", returnLog.TableName, returnLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", returnLog.TableName, returnLog.RecordId);

            var refund = new RequestAddReturnRefundDTO();
            refund.ReturnId = updatedReturn.ReturnId;
            refund.RefundAmount = 0;
            var createdRefund = await _adminRefundService.CreateReturnRefund(refund);
            if (createdRefund == null)
            {
                _logger.LogError("Failed to create refund for ReturnId {ReturnId}", updatedReturn.ReturnId);
                throw new DataRegistrationException("Refund creation failed");
            }
            _logger.LogInformation("Refund created for ReturnId {ReturnId}", updatedReturn.ReturnId);

            // Notify the customer that their return has been accepted and refund initiated
            var customerUserId = returnItem.OrderItems?.Order?.UserId;
            if (customerUserId != null && customerUserId != 0)
            {
                await _notificationService.SendToUser(
                    customerUserId.Value,
                    "Return Accepted",
                    $"Your return has been accepted and your refund is being processed.",
                    notificationTypeId: (int)NotificationTypeEnum.ReturnApproved,
                    referenceType: "Return",
                    referenceId: updatedReturn.ReturnId);
                _logger.LogInformation("Return accepted notification sent to customer UserId {UserId}", customerUserId);
            }
            else
            {
                _logger.LogWarning("No customer UserId found for ReturnId {ReturnId}. Skipping customer notification", updatedReturn.ReturnId);
            }

            await transaction.CommitAsync();

            return _mapper.Map<ResponseReviewReturnDTO>(updatedReturn);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex,
                "Transaction failed while accepting ReturnId {ReturnId}, VendorUserId {VendorUserId}",
                returnId,
                vendorUserId);

            throw;
        }
    }

    public async Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForVendor(RequestVendorReturnFilter request, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested returns with filter {@Filter}", vendorUserId, request);
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var result = await _returnRepsository.GetAllReturnsForVendor(request, vendor.VendorId);
        _logger.LogInformation("Returning {ReturnCount} returns for VendorId {VendorId}", result.data.Count, vendor.VendorId);
        return new PagedResponse<ReturnSummaryDto>
        {
            Items = _mapper.Map<List<ReturnSummaryDto>>(result.data),
            TotalCount = result.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<ReturnDetailsDto?> GetReturnDetails(int returnId)
    {
        var entity = await _returnRepsository.GetReturnDetails(returnId);

        if (entity == null)
            return null;

        return _mapper.Map<ReturnDetailsDto>(entity);
    }
}