using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId)
    {
        var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Vendor review initiated by UserId {UserId} for VendorId {VendorId}", userId, requestReviewOfVendorDTO.VendorId);

            var vendor = await _vendorValidation.ValidateVendor(requestReviewOfVendorDTO.VendorId);
            _logger.LogInformation("VendorId {VendorId} validated. Current ApprovalStatusId {ApprovalStatusId}", vendor.VendorId, vendor.ApprovalStatusId);

            if (vendor.ApprovalStatusId == (int)ApprovalStatusEnum.Accepted || vendor.ApprovalStatusId == (int)ApprovalStatusEnum.Rejected)
            {
                _logger.LogWarning("Vendor review rejected. VendorId {VendorId} already reviewed with ApprovalStatusId {ApprovalStatusId}", vendor.VendorId, vendor.ApprovalStatusId);
                throw new DataApprovalStatusException("Vendor Already Reviewed");
            }

            var adminUser = await _adminUserValidation.ValidateVendorAdminUserByUserId(userId);
            _logger.LogInformation("Admin user {AdminUserId} validated successfully", adminUser.AdminUserId);

            int previousApprovalStatusId = vendor.ApprovalStatusId;

            VendorApprovalHistory approvalHistory = new VendorApprovalHistory();
            approvalHistory.EntityId = vendor.VendorId;
            approvalHistory.PreviousStatusId = vendor.ApprovalStatusId;
            approvalHistory.NewStatusId = requestReviewOfVendorDTO.ApprovalStatusId;
            approvalHistory.Remarks = requestReviewOfVendorDTO.Remark;
            approvalHistory.ReviewerId = adminUser.AdminUserId;
            approvalHistory.ReviewedAt = DateTime.Now;

            vendor.ApprovalStatusId = requestReviewOfVendorDTO.ApprovalStatusId;
            vendor.ReviewedByAdminId = adminUser.AdminUserId;
            vendor.ReviewedAt = DateTime.Now;

            _logger.LogInformation("Updating VendorId {VendorId} with ApprovalStatusId {ApprovalStatusId}", vendor.VendorId, requestReviewOfVendorDTO.ApprovalStatusId);

            var updatedVendor = await _vendorRepsository.Update(requestReviewOfVendorDTO.VendorId, vendor);
            if (updatedVendor == null)
            {
                _logger.LogError("Failed to update VendorId {VendorId}", vendor.VendorId);
                throw new DataRegistrationException("Updation of the vendor failed");
            }
            _logger.LogInformation("VendorId {VendorId} updated successfully. ApprovalStatusId {ApprovalStatusId}", updatedVendor.VendorId, updatedVendor.ApprovalStatusId);

            var createdApprovalHistory = await _approvalHistoryRepsository.Create(approvalHistory);
            if (createdApprovalHistory == null)
            {
                _logger.LogError("Failed to create vendor approval history for VendorId {VendorId}", vendor.VendorId);
                throw new DataRegistrationException("Vendor approval history creation failed.");
            }
            _logger.LogInformation("Vendor approval history created for VendorId {VendorId}. PreviousStatus {PreviousStatus}, NewStatus {NewStatus}", vendor.VendorId, approvalHistory.PreviousStatusId, approvalHistory.NewStatusId);

            var vendorLog = new LogChanges
            {
                TableName = nameof(Vendor),
                RecordId = vendor.VendorId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"VendorId={vendor.VendorId}, ApprovalStatusId={previousApprovalStatusId}",
                NewValue = $"VendorId={updatedVendor.VendorId}, ApprovalStatusId={updatedVendor.ApprovalStatusId}",
                UserId = userId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(vendorLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", vendorLog.TableName, vendorLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", vendorLog.TableName, vendorLog.RecordId);

            _logger.LogInformation("VendorId {VendorId} reviewed successfully by AdminUserId {AdminUserId}", vendor.VendorId, adminUser.AdminUserId);

            var ownerUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendor.VendorId);
            if (ownerUser != null)
            {
                _logger.LogInformation("Sending vendor review notification to UserId {VendorOwnerUserId}", ownerUser.UserId);
                await _notificationService.SendToUser(
                    ownerUser.UserId,
                    requestReviewOfVendorDTO.ApprovalStatusId == (int)ApprovalStatusEnum.Accepted ? "Vendor Approved" : "Vendor Rejected",
                    requestReviewOfVendorDTO.Remark ?? "",
                    notificationTypeId: requestReviewOfVendorDTO.ApprovalStatusId == (int)ApprovalStatusEnum.Accepted ? 15 : 16,
                    referenceType: "Vendor",
                    referenceId: vendor.VendorId);
                _logger.LogInformation("Vendor review notification sent to UserId {VendorOwnerUserId}", ownerUser.UserId);
            }
            else
            {
                _logger.LogWarning("Vendor owner user not found for VendorId {VendorId}", vendor.VendorId);
            }

            await transaction.CommitAsync();
            _logger.LogInformation("Vendor review process completed for VendorId {VendorId}", vendor.VendorId);

            return _mapper.Map<ResponseReviewOfVendorDTO>(vendor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing transaction");
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for VendorId {VendorId}", requestReviewOfVendorDTO.VendorId);
            throw;
        }
    }
}