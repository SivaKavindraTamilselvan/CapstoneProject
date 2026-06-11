using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<ResponseReviewOfVendorDTO> DeleteVendor(int vendorId, int adminUserId)
    {
        var adminUser = await _adminUserRepsository.GetAdminUserByUserId(adminUserId);
        if (adminUser == null)
        {
            _logger.LogError("Admin record not found for UserId {UserId}", adminUserId);
            throw new DataNotFoundException("Admin Not Found");
        }
        var vendor = await _vendorRepsository.Get(vendorId);
        if (vendor == null)
        {
            _logger.LogWarning("Vendor review failed. VendorId {VendorId} not found", vendorId);
            throw new DataNotFoundException("Vendor ID Not Found");
        }
        vendor.ApprovalStatusId = 4;
        vendor.ReviewedAt = DateTime.Now;
        await _vendorRepsository.Update(vendorId, vendor);
        return _mapper.Map<ResponseReviewOfVendorDTO>(vendor);
    }
}