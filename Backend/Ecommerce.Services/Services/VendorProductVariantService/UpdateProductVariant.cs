using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
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
    public async Task<ResponseUpdateProductVariantDTO> UpdateRejectedProductVariant(RequestUpdateProductVariant requestUpdateProductVariantDTO,int vendoruserId)
    {
        var productVariant = await _productValidation.ValidateProductVariant(requestUpdateProductVariantDTO.ProductVariantId,vendoruserId);
        if(productVariant.ProductApprovalStatusId == 4 || productVariant.ProductApprovalStatusId == 6)
        {
            throw new DataApprovalStatusException("Product Variant Cannot be updated if admin approved");
        }
        var updatedProductVariant = _mapper.Map<ProductVariant>(productVariant);
        updatedProductVariant.UpdatedAt = DateTime.Now;
        await _productVariantRepsository.Update(productVariant.ProductVariantId,updatedProductVariant);
        return _mapper.Map<ResponseUpdateProductVariantDTO>(updatedProductVariant);
    }
}