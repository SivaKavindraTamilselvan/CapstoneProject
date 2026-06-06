using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class VendorProductImageService : IVendorProductImageService
{
    public async Task<ResponseAddProductImage> AddProductImage(RequestAddProductImage requestAddProductImage, int vendorUserId)
    {
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var product = await _productValidation.ValidateProduct(requestAddProductImage.ProductId);
        if (requestAddProductImage.ProductVariantId != null)
        {
            var productVariant = await _productValidation.ValidateProductVariant(requestAddProductImage.ProductVariantId.Value);
        }
        var productImage = _mapper.Map<ProductImage>(requestAddProductImage);
        productImage.AddedByVendorUserId = vendorUser.VendorUserId;
        await _productImageRepsository.Create(productImage);
        return _mapper.Map<ResponseAddProductImage>(productImage);
    }

}