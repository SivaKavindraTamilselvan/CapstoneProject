using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class VendorProductImageService : IVendorProductImageService
{
    public async Task<ResponseAddProductImage> AddProductImage(RequestAddProductImage requestAddProductImage, int vendorUserId)
    {
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var product = await _productValidation.ValidateProduct(requestAddProductImage.ProductId);
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
        var productImageList = await _productImageRepsository.GetAllProductImageByProductId(image.ProductId);
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
    public async Task<ResponseAddProductVariantImage> AddProductVariantImage(RequestAddProductVariantImage requestAddProductVariantImage, int vendorUserId)
    {
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var productVariant = await _productValidation.ValidateProductVariant(requestAddProductVariantImage.ProductVariantId);
        await _productValidation.ValidateProductIfApproved(productVariant.ProductId);
        var productImage = _mapper.Map<ProductImage>(requestAddProductVariantImage);
        productImage.AddedByVendorUserId = vendorUser.VendorUserId;
        await _productImageRepsository.Create(productImage);
        return _mapper.Map<ResponseAddProductVariantImage>(productImage);
    }
}