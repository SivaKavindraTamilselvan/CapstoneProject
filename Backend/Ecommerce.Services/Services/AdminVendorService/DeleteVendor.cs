using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<ResponseReviewOfVendorDTO> DeleteVendor(DeleteVendorDto deleteVendorDto, int adminUserId)
    {
        _logger.LogInformation("DeleteVendor started for VendorId {VendorId} by AdminUserId {AdminUserId}", deleteVendorDto.VendorId, adminUserId);

        var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var adminUser = await _adminUserValidation.ValidateVendorAdminUserByUserId(adminUserId);
            _logger.LogInformation("Admin user {AdminUserId} validated successfully", adminUserId);

            var vendor = await _vendorRepsository.Get(deleteVendorDto.VendorId);
            if (vendor == null)
            {
                _logger.LogWarning("Vendor review failed. VendorId {VendorId} not found", deleteVendorDto.VendorId);
                throw new DataNotFoundException("Vendor ID Not Found");
            }
            _logger.LogInformation("VendorId {VendorId} found. Current ApprovalStatusId {ApprovalStatusId}", vendor.VendorId, vendor.ApprovalStatusId);

            int previousApprovalStatusId = vendor.ApprovalStatusId;

            VendorApprovalHistory approvalHistory = new VendorApprovalHistory();
            approvalHistory.EntityId = vendor.VendorId;
            approvalHistory.PreviousStatusId = vendor.ApprovalStatusId;
            approvalHistory.NewStatusId = (int)ApprovalStatusEnum.Deleted_By_Admin;
            approvalHistory.Remarks = deleteVendorDto.Remark;
            approvalHistory.ReviewerId = adminUser.AdminUserId;
            approvalHistory.ReviewedAt = DateTime.Now;

            var createdApprovalHistory = await _approvalHistoryRepsository.Create(approvalHistory);
            if (createdApprovalHistory == null)
            {
                _logger.LogError("Failed to create vendor approval history for VendorId {VendorId}", vendor.VendorId);
                throw new DataRegistrationException("Vendor approval history creation failed.");
            }
            _logger.LogInformation("Vendor approval history created for VendorId {VendorId}. PreviousStatus {PreviousStatus}, NewStatus {NewStatus}", vendor.VendorId, approvalHistory.PreviousStatusId, approvalHistory.NewStatusId);

            vendor.ApprovalStatusId = (int)ApprovalStatusEnum.Deleted_By_Admin;
            vendor.ReviewedAt = DateTime.Now;
            vendor.ReviewedByAdminId = adminUser.AdminUserId;

            var ownerVendorUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendor.VendorId);
            if (ownerVendorUser != null)
            {
                _logger.LogInformation("Sending vendor deletion notification to UserId {UserId} for VendorId {VendorId}", ownerVendorUser.UserId, vendor.VendorId);
                await _notificationService.SendToUser(
                    ownerVendorUser.UserId,
                    "Vendor Deleted",
                    deleteVendorDto.Remark,
                    (int)NotificationTypeEnum.VendorDeleted,
                    "Vendor",
                    vendor.VendorId
                );
                _logger.LogInformation("Vendor deletion notification sent to UserId {UserId}", ownerVendorUser.UserId);
            }
            else
            {
                _logger.LogWarning("No owner vendor user found for VendorId {VendorId}. Skipping notification", vendor.VendorId);
            }

            var updatedVendor = await _vendorRepsository.Update(deleteVendorDto.VendorId, vendor);
            if (updatedVendor == null)
            {
                _logger.LogError("Failed to update VendorId {VendorId}", vendor.VendorId);
                throw new DataRegistrationException("Updation of the vendor failed");
            }
            _logger.LogInformation("VendorId {VendorId} updated successfully. ApprovalStatusId {ApprovalStatusId}", updatedVendor.VendorId, updatedVendor.ApprovalStatusId);

            var vendorLog = new LogChanges
            {
                TableName = nameof(Vendor),
                RecordId = vendor.VendorId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"VendorId={vendor.VendorId}, ApprovalStatusId={previousApprovalStatusId}",
                NewValue = $"VendorId={updatedVendor.VendorId}, ApprovalStatusId={updatedVendor.ApprovalStatusId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(vendorLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", vendorLog.TableName, vendorLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", vendorLog.TableName, vendorLog.RecordId);

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for VendorId {VendorId}", vendor.VendorId);

            return _mapper.Map<ResponseReviewOfVendorDTO>(vendor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing transaction");
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for VendorId {VendorId}", deleteVendorDto.VendorId);
            throw;
        }
    }
}