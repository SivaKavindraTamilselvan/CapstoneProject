using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductImageService : IVendorProductImageService
{
    public async Task<ResponseMakeDefaultImageDTO> DeleteProductImage(int productImageId, int userid)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested deletion of ProductImageId {ProductImageId}",userid,productImageId);
        var product = await _productImageRepsository.Get(productImageId);
        if (product == null)
        {
            _logger.LogWarning("Vendor UserId {VendorUserId} attempted to delete main image ProductImageId {ProductImageId}",userid,productImageId);
            throw new DataNotFoundException("Product Image Is Not Found");
        }
        if (product.IsMainImage)
        {
            _logger.LogWarning("Vendor UserId {VendorUserId} attempted to delete main image ProductImageId {ProductImageId}",userid,productImageId);
            throw new DataApprovalStatusException("Main Image Cannot Be Deleted");
        }
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(userid);
        await _productValidation.VendorValidateProduct(product.ProductId, vendorUser.VendorId);
        await _productImageRepsository.Delete(productImageId);
        _logger.LogInformation("ProductImageId {ProductImageId} deleted successfully by Vendor UserId {VendorUserId}",productImageId,userid);
        return _mapper.Map<ResponseMakeDefaultImageDTO>(product);
    }
}