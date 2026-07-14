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
            if (vendor.ApprovalStatusId == (int)ApprovalStatusEnum.Accepted || vendor.ApprovalStatusId == (int)ApprovalStatusEnum.Rejected)
            {
                throw new DataApprovalStatusException("Vendor Already Reviewed");
            }
            var adminUser = await _adminUserValidation.ValidateAdminUserByUserId(userId);
            
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

            await _vendorRepsository.Update(requestReviewOfVendorDTO.VendorId, vendor);
            await _approvalHistoryRepsository.Create(approvalHistory);

            _logger.LogInformation("VendorId {VendorId} reviewed successfully by AdminUserId {AdminUserId}", vendor.VendorId, adminUser.AdminUserId);

            var ownerUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendor.VendorId);
            if (ownerUser != null)
            {
                await _notificationService.SendToUser(
                    ownerUser.UserId,
                    requestReviewOfVendorDTO.ApprovalStatusId == (int)ApprovalStatusEnum.Accepted ? "Vendor Approved" : "Vendor Rejected",
                    requestReviewOfVendorDTO.Remark ?? "",
                    notificationTypeId: requestReviewOfVendorDTO.ApprovalStatusId == (int)ApprovalStatusEnum.Accepted ? 15 : 16,
                    referenceType: "Vendor",
                    referenceId: vendor.VendorId);
                _logger.LogInformation("Sending vendor review notification to UserId {VendorOwnerUserId}", ownerUser.UserId);
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
            throw;
        }
    }

}