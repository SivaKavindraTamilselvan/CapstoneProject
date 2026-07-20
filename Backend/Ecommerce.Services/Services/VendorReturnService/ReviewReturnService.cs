using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorReturnService : IVendorReturnService
{
    private readonly EcommerceContext _ecommerceContext;
    private readonly IAdminRefundService _adminRefundService;
    private readonly IAdminReturnService _adminReturnService;
    private readonly ILogger<VendorReturnService> _logger;
    private readonly IReturnRepsository _returnRepsository;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IMapper _mapper;
    private readonly ILogChanges _logChanges;
    private readonly INotificationService _notificationService;

    public VendorReturnService(
        EcommerceContext ecommerceContext,
        IAdminRefundService adminRefundService,
        IAdminReturnService adminReturnService,
        ILogger<VendorReturnService> logger,
        IReturnRepsository returnRepsository,
        IMapper mapper,
        IVendorUserValidation vendorUserValidation,
        ILogChanges logChanges,
        INotificationService notificationService)
    {
        _ecommerceContext = ecommerceContext;
        _adminRefundService = adminRefundService;
        _adminReturnService = adminReturnService;
        _returnRepsository = returnRepsository;
        _vendorUserValidation = vendorUserValidation;
        _mapper = mapper;
        _logger = logger;
        _logChanges = logChanges;
        _notificationService = notificationService;
    }

    public async Task<ResponseReviewReturnDTO> ReviewReturnOrder(RequestReviewReturnDTO requestReviewReturnDTO, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {

            _logger.LogInformation("Vendor UserId {VendorUserId} reviewing ReturnId {ReturnId}. Review Decision: {Review}", vendorUserId, requestReviewReturnDTO.ReturnId, requestReviewReturnDTO.Review);

            var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            var returnItem = await _returnRepsository.Get(requestReviewReturnDTO.ReturnId);
            if (returnItem == null)
            {
                _logger.LogWarning("ReturnId {ReturnId} not found for Vendor UserId {VendorUserId}", requestReviewReturnDTO.ReturnId, vendorUserId);
                throw new DataNotFoundException("Return Not Found");
            }

            // capture the previous status BEFORE overwriting it
            int previousStatus = returnItem.ReturnStatusId;

            if (requestReviewReturnDTO.Review)
            {
                returnItem.ReturnStatusId = (int)ReturnStatusEnum.Approved;
            }
            else
            {
                returnItem.ReturnStatusId = (int)ReturnStatusEnum.Rejected;
            }
            returnItem.ReviewedByVendorId = vendorUser.VendorUserId;

            var updatedReturn = await _returnRepsository.Update(returnItem.ReturnId, returnItem);
            if (updatedReturn == null)
            {
                _logger.LogError("Failed to update ReturnId {ReturnId}", returnItem.ReturnId);
                throw new DataRegistrationException("Updation of the return failed");
            }
            _logger.LogInformation("ReturnId {ReturnId} reviewed successfully by Vendor UserId {VendorUserId}. Status changed from {PreviousStatus} to {NewStatus}", updatedReturn.ReturnId, vendorUserId, previousStatus, updatedReturn.ReturnStatusId);

            var returnLog = new LogChanges
            {
                TableName = nameof(Return),
                RecordId = updatedReturn.ReturnId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ReturnId={returnItem.ReturnId}, ReturnStatusId={previousStatus}",
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

            var createdShipment = await _adminReturnService.CreateReturnShipment(updatedReturn.ReturnId);
            if (createdShipment == null)
            {
                _logger.LogError("Failed to create return shipment for ReturnId {ReturnId}", updatedReturn.ReturnId);
                throw new DataRegistrationException("Return shipment creation failed");
            }
            _logger.LogInformation("Return shipment created for ReturnId {ReturnId}", updatedReturn.ReturnId);

            // Notify the customer of the vendor's review decision
            var customerUserId = returnItem.OrderItems?.Order?.UserId;
            if (customerUserId != null && customerUserId != 0)
            {
                string title = requestReviewReturnDTO.Review ? "Return Approved" : "Return Rejected";
                string message = requestReviewReturnDTO.Review
                    ? "Your return request has been approved. A return shipment has been arranged."
                    : "Your return request has been rejected by the vendor.";

                await _notificationService.SendToUser(
                    customerUserId.Value,
                    title,
                    message,
                    notificationTypeId: requestReviewReturnDTO.Review ? (int)NotificationTypeEnum.ReturnApproved : (int)NotificationTypeEnum.ReturnRejected,
                    referenceType: "Return",
                    referenceId: updatedReturn.ReturnId);
                _logger.LogInformation("Return review notification sent to customer UserId {UserId}", customerUserId);
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
                requestReviewReturnDTO.ReturnId,
                vendorUserId);

            throw;
        }
    }
}