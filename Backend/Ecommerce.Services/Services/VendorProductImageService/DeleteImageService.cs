using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class VendorProductImageService : IVendorProductImageService
{
    public async Task<ResponseMakeDefaultImageDTO> DeleteProductImage(int productImageId, int userid)
    {
        var product = await _productImageRepsository.Get(productImageId);
        if (product == null)
        {
            throw new DataNotFoundException("Product Image Is Not Found");
        }
        if (product.IsMainImage)
        {
            throw new DataApprovalStatusException("Main Image Cannot Be Deleted");
        }
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(userid);
        await _productValidation.VendorValidateProduct(product.ProductId, vendorUser.VendorId);
        await _productImageRepsository.Delete(productImageId);
        return _mapper.Map<ResponseMakeDefaultImageDTO>(product);
    }
}