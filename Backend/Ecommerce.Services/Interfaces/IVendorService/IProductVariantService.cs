using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorProductVariantService
{
    public Task<ResponseAddProductVariantAttributeDTO> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO,bool updation);
    public Task<ResponseUpdateProductVariantDTO> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO);
    public Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO,int vendorUserId);
}