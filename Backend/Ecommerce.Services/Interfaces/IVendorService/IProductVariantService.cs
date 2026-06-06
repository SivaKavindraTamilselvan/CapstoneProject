using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorProductVariantService
{
    public Task<ResponseUpdateProductVariantDTO> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO);
    public Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO,int vendorUserId);
}