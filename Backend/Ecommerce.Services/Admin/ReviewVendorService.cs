using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminService : IAdminService
{
    public async Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO,int userId)
    {
        var vendor = await _vendorRepsository.Get(requestReviewOfVendorDTO.VendorId);
        if(vendor == null)
        {
            throw new DataNotFoundException("Vendor ID Not Found");
        }
        var adminUser = (await _adminUserRepsository.GetAll()).FirstOrDefault(u=>u.UserId == userId);
        if(adminUser == null)
        {
            throw new DataNotFoundException("Admin Not Found");
        }
        vendor.ApprovalStatusId = requestReviewOfVendorDTO.ApprovalStatusId;
        vendor.ReviewedByAdminId = adminUser.AdminUserId;
        vendor.ReviewedAt = DateTime.Now;
        await _vendorRepsository.Update(requestReviewOfVendorDTO.VendorId,vendor);        
        return new ResponseReviewOfVendorDTO
        {
            VendorId = vendor.VendorId,
            ApprovalStatusId = vendor.ApprovalStatusId,
            ReviewedByAdminId = vendor.ReviewedByAdminId,
            ReviewedAt = vendor.ReviewedAt
        };
    }
}