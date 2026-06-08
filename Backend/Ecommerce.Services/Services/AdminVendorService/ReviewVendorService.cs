using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<List<ResponseGetVendor>> GetAllPendingVendor()
    {
        _logger.LogInformation("Fetching all pending vendors");
        var vendor = await _vendorRepsository.GetAllVednorNotPendingApproval();
        if (vendor.Count == 0)
        {
            _logger.LogWarning("No pending vendors found");
            throw new DataNotFoundException("No Pending Vendors");
        }
        _logger.LogInformation("{Count} pending vendors found", vendor.Count);
        return _mapper.Map<List<ResponseGetVendor>>(vendor);
    }
    public async Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId)
    {
        _logger.LogInformation("Vendor review initiated by UserId {UserId} for VendorId {VendorId}", userId, requestReviewOfVendorDTO.VendorId);
        var vendor = await _vendorRepsository.Get(requestReviewOfVendorDTO.VendorId);
        if (vendor == null)
        {
            _logger.LogWarning("Vendor review failed. VendorId {VendorId} not found", requestReviewOfVendorDTO.VendorId);
            throw new DataNotFoundException("Vendor ID Not Found");
        }
        var adminUser = await _adminUserRepsository.GetAdminUserByUserId(userId);
        if (adminUser == null)
        {
            _logger.LogError("Admin record not found for UserId {UserId}", userId);
            throw new DataNotFoundException("Admin Not Found");
        }
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
            _logger.LogInformation("Sending vendor review notification to UserId {VendorOwnerUserId}", vendorOwnerUserId);

            await _notificationService.SendToUser(
                vendorOwnerUserId.Value.ToString(),
                new
                {
                    message = message,
                    vendorId = vendor.VendorId,
                    status = requestReviewOfVendorDTO.ApprovalStatusId == 2 ? "Approved" : "Rejected"
                }
            );
        }
        else
        {
            _logger.LogWarning("Vendor owner user not found for VendorId {VendorId}", vendor.VendorId);
        }

        _logger.LogInformation("Vendor review process completed for VendorId {VendorId}", vendor.VendorId);
        return _mapper.Map<ResponseReviewOfVendorDTO>(vendor);
    }
}