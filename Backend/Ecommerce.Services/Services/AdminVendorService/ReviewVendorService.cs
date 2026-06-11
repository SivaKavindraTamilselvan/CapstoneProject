using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId)
    {
        _logger.LogInformation("Vendor review initiated by UserId {UserId} for VendorId {VendorId}", userId, requestReviewOfVendorDTO.VendorId);
        var vendor = await _vendorValidation.ValidateVendor(requestReviewOfVendorDTO.VendorId);
        if (vendor.ApprovalStatusId == 2 || vendor.ApprovalStatusId == 3)
        {
            throw new DataApprovalStatusException("Vendor Already Reviewed");
        }
        var adminUser = await _adminUserValidation.ValidateAdminUserByUserId(userId);
        vendor.ApprovalStatusId = requestReviewOfVendorDTO.ApprovalStatusId;
        vendor.ReviewedByAdminId = adminUser.AdminUserId;
        vendor.ReviewedAt = DateTime.Now;
        _logger.LogInformation("Updating VendorId {VendorId} with ApprovalStatusId {ApprovalStatusId}", vendor.VendorId, requestReviewOfVendorDTO.ApprovalStatusId);
        await _vendorRepsository.Update(requestReviewOfVendorDTO.VendorId, vendor);
        _logger.LogInformation("VendorId {VendorId} reviewed successfully by AdminUserId {AdminUserId}", vendor.VendorId, adminUser.AdminUserId);
        string message = "";
        if (requestReviewOfVendorDTO.ApprovalStatusId == 2)
        {
            message = "Your vendor account has been approved!";
        }
        else
        {
            message = "Your vendor account has been rejected.";
        }
        var ownerUser = await _vendorRepsository.GetOwnerVendorUserByVendorId(vendor.VendorId);
        var vendorOwnerUserId = ownerUser?.VendorUsers?.FirstOrDefault()?.UserId;
        if (vendorOwnerUserId.HasValue)
        {
            await _notificationService.SendToUser(
                vendorOwnerUserId.Value,
                requestReviewOfVendorDTO.ApprovalStatusId == 2 ? "Vendor Approved" : "Vendor Rejected",
                message,
                notificationTypeId: requestReviewOfVendorDTO.ApprovalStatusId == 2 ? 15 : 16,
                referenceType: "Vendor",
                referenceId: vendor.VendorId);
            _logger.LogInformation("Sending vendor review notification to UserId {VendorOwnerUserId}", vendorOwnerUserId.Value);
        }
        else
        {
            _logger.LogWarning("Vendor owner user not found for VendorId {VendorId}", vendor.VendorId);
        }

        _logger.LogInformation("Vendor review process completed for VendorId {VendorId}", vendor.VendorId);
        return _mapper.Map<ResponseReviewOfVendorDTO>(vendor);
    }

}