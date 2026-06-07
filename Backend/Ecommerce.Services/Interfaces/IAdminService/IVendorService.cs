using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminVendorService
{
    public Task<List<ResponseGetVendor>> GetAllPendingVendor();
    public Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId);
}