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
        if(requestAddProductImage.IsMainImage == true)
        {
            RequestMakeDefaultImageDTO requestMakeDefaultImageDTO = new RequestMakeDefaultImageDTO();
            requestMakeDefaultImageDTO.ProductImageId = productImage.ProductImageId;
            await MakeImageDefault(requestMakeDefaultImageDTO);
        }
        return _mapper.Map<ResponseAddProductImage>(productImage);
    }
    public async Task<ResponseMakeDefaultImageDTO> MakeImageDefault(RequestMakeDefaultImageDTO requestMakeDefaultImageDTO)
    {
        var image = await _productValidation.ValidateProductImage(requestMakeDefaultImageDTO.ProductImageId);
        var product = await _productValidation.ValidateProductVariant(image.ProductId);
        var productImageList = await _productImageRepsository.GetAllProductImageByProductId(product.ProductId);
        foreach(var images in productImageList)
        {
            images.IsMainImage = false;
            images.UpdatedAt = DateTime.Now;
            await _productImageRepsository.Update(images.ProductImageId,images);
        }
        image.IsMainImage = true;
        image.UpdatedAt = DateTime.Now;
        await _productImageRepsository.Update(image.ProductImageId,image);
        return _mapper.Map<ResponseMakeDefaultImageDTO>(image);
    }
}