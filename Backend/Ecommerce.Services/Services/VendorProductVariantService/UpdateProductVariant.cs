using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    public async Task<ResponseUpdateProductVariantDTO> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO,int vendoruserId)
    {
        var productVariant = await _productValidation.ValidateProductVariant(requestUpdateProductVariantDTO.ProductVariantId,vendoruserId);
        productVariant.UpdatedAt = DateTime.Now;
        productVariant.ProductVariantStatusId = requestUpdateProductVariantDTO.ProductStatusId;
        await _productVariantRepsository.Update(productVariant.ProductVariantId,productVariant);
        return _mapper.Map<ResponseUpdateProductVariantDTO>(productVariant);
    }
}