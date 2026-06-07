using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<List<ResponseGetVendor>> GetAllPendingVendor()
    {
        var vendor = await _vendorRepsository.GetAllVednorNotPendingApproval();
        if (vendor.Count == 0)
        {
            throw new DataNotFoundException("No Pedning Vendors");
        }
        return _mapper.Map<List<ResponseGetVendor>>(vendor);
    }
    public async Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId)
    {
        var vendor = await _vendorRepsository.Get(requestReviewOfVendorDTO.VendorId);
        if (vendor == null)
        {
            throw new DataNotFoundException("Vendor ID Not Found");
        }
        var adminUser = (await _adminUserRepsository.GetAll()).FirstOrDefault(u => u.UserId == userId);
        if (adminUser == null)
        {
            throw new DataNotFoundException("Admin Not Found");
        }
        vendor.ApprovalStatusId = requestReviewOfVendorDTO.ApprovalStatusId;
        vendor.ReviewedByAdminId = adminUser.AdminUserId;
        vendor.ReviewedAt = DateTime.Now;
        await _vendorRepsository.Update(requestReviewOfVendorDTO.VendorId, vendor);
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
                vendorOwnerUserId.Value.ToString(),
                new
                {
                    message = message,
                    vendorId = vendor.VendorId,
                    status = requestReviewOfVendorDTO.ApprovalStatusId == 2 ? "Approved" : "Rejected"
                }
            );
        }
        return _mapper.Map<ResponseReviewOfVendorDTO>(vendor);
    }
}