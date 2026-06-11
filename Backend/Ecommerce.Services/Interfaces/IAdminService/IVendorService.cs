using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminVendorService
{
    public Task<ResponseReviewOfVendorDTO> DeleteVendor(int vendorId,int adminUserId);
    public Task<List<ResponseAdminGetVendorDTO>> GetVendorsForAdmin(RequestAdminVendorFilter request);
    public Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId);
}