using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    public async Task<ResponseUpdateProductVariantDTO> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO,int vendoruserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} updating ProductVariantId {ProductVariantId}",vendoruserId,requestUpdateProductVariantDTO.ProductVariantId);

        var productVariant = await _productValidation.ValidateProductVariant(requestUpdateProductVariantDTO.ProductVariantId,vendoruserId);
        productVariant.UpdatedAt = DateTime.Now;
        productVariant.ProductVariantStatusId = requestUpdateProductVariantDTO.ProductStatusId;
        await _productVariantRepsository.Update(productVariant.ProductVariantId,productVariant);
        _logger.LogInformation("ProductVariantId {ProductVariantId} updated successfully by Vendor UserId {VendorUserId}",productVariant.ProductVariantId,vendoruserId);
        return _mapper.Map<ResponseUpdateProductVariantDTO>(productVariant);
    }
    public async Task<ResponseUpdateProductVariantDTO> UpdateRejectedProductVariant(RequestUpdateProductVariant requestUpdateProductVariantDTO,int vendoruserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} updating rejected ProductVariantId {ProductVariantId}",vendoruserId,requestUpdateProductVariantDTO.ProductVariantId);

        var productVariant = await _productValidation.ValidateProductVariant(requestUpdateProductVariantDTO.ProductVariantId,vendoruserId);
        if(productVariant.ProductApprovalStatusId == 4 || productVariant.ProductApprovalStatusId == 6)
        {
            _logger.LogWarning("ProductVariantId {ProductVariantId} cannot be updated because ApprovalStatusId is {ApprovalStatusId}",productVariant.ProductVariantId,productVariant.ProductApprovalStatusId);
            throw new DataApprovalStatusException("Product Variant Cannot be updated if admin approved");
        }
        var updatedProductVariant = _mapper.Map<ProductVariant>(productVariant);
        updatedProductVariant.UpdatedAt = DateTime.Now;
        await _productVariantRepsository.Update(productVariant.ProductVariantId,updatedProductVariant);
         _logger.LogInformation("Rejected ProductVariantId {ProductVariantId} updated successfully by Vendor UserId {VendorUserId}",productVariant.ProductVariantId,vendoruserId);
        return _mapper.Map<ResponseUpdateProductVariantDTO>(updatedProductVariant);
    }
}