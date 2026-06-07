using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<List<ResponseGetVendor>> GetAllPendingVendor()
    {
        var vendor = await _vendorRepsository.GetAllVednorNotPendingApproval();
        if(vendor.Count == 0)
        {
            throw new DataNotFoundException("No Pedning Vendors");
        }
        return _mapper.Map<List<ResponseGetVendor>>(vendor);
    }
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
        return _mapper.Map<ResponseReviewOfVendorDTO>(vendor);
    }
}