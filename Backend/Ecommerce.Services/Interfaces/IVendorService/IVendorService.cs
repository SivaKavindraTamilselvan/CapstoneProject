using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorService
{
    public Task<ResponseReviewOfProductDTO> ReviewProductByVendor(RequestReviewOfProductDTO requestReviewOfProductDTO, int vendorUserId);
    public Task<ResponseRegisterVendorUserDTO> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO, int vendorUserId);
}