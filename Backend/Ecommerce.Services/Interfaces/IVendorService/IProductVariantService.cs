using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorProductVariantService
{
    public Task<ResponseUpdateProductVariantDTO> UpdateRejectedProductVariant(RequestUpdateProductVariant requestUpdateProductVariantDTO,int vendoruserId);
    public Task<ResponseAddProductVariantAttributeDTO> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO,bool updation,int vendorUserId);
    public Task<ResponseUpdateProductVariantDTO> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO,int vendorUserId);
    public Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO,int vendorUserId);
}