using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorProductImageService
{
    public Task<ResponseAddProductVariantImage> AddProductVariantImage(RequestAddProductVariantImage requestAddProductVariantImage, int vendorUserId);
    public Task<ResponseMakeDefaultImageDTO> MakeImageDefault(RequestMakeDefaultImageDTO requestMakeDefaultImageDTO);
    public Task<ResponseAddProductImage> AddProductImage(RequestAddProductImage requestAddProductImage,int vendorUserId);
}