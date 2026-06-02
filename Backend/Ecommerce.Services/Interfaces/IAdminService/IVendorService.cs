using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminVendorService
{
    public Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId);
}