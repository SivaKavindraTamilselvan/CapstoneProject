using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminVendorService
{
    public Task<ResponseReviewOfVendorDTO> DeleteVendor(int vendorId,int adminUserId);
    public Task<List<ResponseGetVendor>> GetVendor(int? statusId,int pageNumber,int pageSize);
    public Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId);
}