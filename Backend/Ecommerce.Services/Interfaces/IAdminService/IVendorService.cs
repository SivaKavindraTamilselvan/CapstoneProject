using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminVendorService
{
    public  Task<PagedResponse<ResponseGetVendorUserDTO>> GetVendorUserByVendorIdForAdmin(RequestAdminVendorUserFilter request, int adminUserId);
    public Task<ResponseAdminGetVendorDTO> GetVendorsByVendorIdForAdmin(int vendorId,int adminUserId);
    public Task<ResponseReviewOfVendorDTO> DeleteVendor(DeleteVendorDto deleteVendorDto, int adminUserId);
    public Task<PagedResponse<ResponseAdminGetVendorDTO>> GetVendorsForAdmin(RequestAdminVendorFilter request,int adminUserId);
    public Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId);
}