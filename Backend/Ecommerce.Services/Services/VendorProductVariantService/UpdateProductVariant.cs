using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    public async Task<ResponseUpdateProductVariantDTO> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO)
    {
        var productVariant = await _productValidation.ValidateProductVariant(requestUpdateProductVariantDTO.ProductVariantId);
        var product = await _productValidation.ValidateProductIfApproved(requestUpdateProductVariantDTO.ProductId);
        productVariant.ProductApprovalStatusId = 2;
        productVariant.UpdatedAt = DateTime.Now;
        await _productVariantRepsository.Update(productVariant.ProductVariantId,productVariant);
        return _mapper.Map<ResponseUpdateProductVariantDTO>(productVariant);
    }
}