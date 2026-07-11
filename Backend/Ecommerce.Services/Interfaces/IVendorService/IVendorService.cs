using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorService
{
    public Task<ResponseGetVendorUserDTO> DeactivateAdminUser(int adminUserId, int logedusedId);
    public Task<ResponseGetVendorUserDTO> ActivateAdminUser(int adminUserId, int logedusedId);
    public Task<PagedResponse<ResponseGetVendorUserDTO>> GetAllVendorUser(RequestVendorUserFilter request, int logedusedId);
    public Task<ResponseGetVendorUserDTO> GetVendorUserByUserId(int userId, int logedusedId);
    public Task<ResponseReviewOfProductVariantDTO> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO, int vendorUserId);
    public Task<ResponseReviewOfProductDTO> ReviewProductByVendor(RequestReviewOfProductDTO requestReviewOfProductDTO, int vendorUserId);
    public Task<ResponseRegisterVendorUserDTO> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO, int vendorUserId);
}