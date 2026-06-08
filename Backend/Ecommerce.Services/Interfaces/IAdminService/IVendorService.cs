using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminVendorService
{
    public Task<List<ResponseGetVendor>> GetVendor(int? statusId,int pageNumber,int pageSize);
    public Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId);
}