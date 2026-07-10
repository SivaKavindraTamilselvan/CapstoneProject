using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductImageService : IVendorProductImageService
{
    public async Task<ResponseAddProductImage> AddProductImage(RequestAddProductImage requestAddProductImage, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} started adding product image for ProductId {ProductId}", vendorUserId, requestAddProductImage.ProductId);

        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var product = await _productValidation.VendorValidateProduct(requestAddProductImage.ProductId,vendorUser.VendorId);
        _logger.LogInformation("Product {ProductId} validated for image upload", product.ProductId);

        var image = await _productImageRepsository.GetProductImageByImageURL(requestAddProductImage.ImageUrl);
        if (image != null)
        {
            _logger.LogWarning("Duplicate image URL found while adding product image for ProductId {ProductId}", requestAddProductImage.ProductId);
            throw new DataAlreadyRegisteredException("Image already added");
        }
        var productImage = _mapper.Map<ProductImage>(requestAddProductImage);
        productImage.AddedByVendorUserId = vendorUser.VendorUserId;
        await _productImageRepsository.Create(productImage);

        _logger.LogInformation("Product image {ProductImageId} added successfully for ProductId {ProductId}", productImage.ProductImageId, productImage.ProductId);
        if (requestAddProductImage.IsMainImage == true)
        {
            _logger.LogInformation("Setting ProductImageId {ProductImageId} as default image for ProductId {ProductId}", productImage.ProductImageId, productImage.ProductId);
            RequestMakeDefaultImageDTO requestMakeDefaultImageDTO = new RequestMakeDefaultImageDTO();
            requestMakeDefaultImageDTO.ProductImageId = productImage.ProductImageId;
            await MakeImageDefault(requestMakeDefaultImageDTO);
        }

        var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();
        foreach (var adminUserId in productAdminUserIds)
        {
            await _notificationService.SendToUser(
                adminUserId,
                "New Product Image Added",
                $"A new image has been added for product '{product.ProductName}' and may require review.",
                notificationTypeId: 1,
                referenceType: "ProductImage",
                referenceId: productImage.ProductImageId);
        }

        _logger.LogInformation("Product image notifications sent successfully for ProductImageId {ProductImageId}", productImage.ProductImageId);
        return _mapper.Map<ResponseAddProductImage>(productImage);
    }
    public async Task<ResponseMakeDefaultImageDTO> MakeImageDefault(RequestMakeDefaultImageDTO requestMakeDefaultImageDTO)
    {
        var image = await _productValidation.ValidateProductImage(requestMakeDefaultImageDTO.ProductImageId);
        var productImageList = await _productImageRepsository.GetAllProductImageByProductId(image.ProductId);
        foreach (var images in productImageList)
        {
            images.IsMainImage = false;
            images.UpdatedAt = DateTime.Now;
            await _productImageRepsository.Update(images.ProductImageId, images);
        }
        image.IsMainImage = true;
        image.UpdatedAt = DateTime.Now;
        await _productImageRepsository.Update(image.ProductImageId, image);
        return _mapper.Map<ResponseMakeDefaultImageDTO>(image);
    }
    public async Task<ResponseAddProductVariantImage> AddProductVariantImage(RequestAddProductVariantImage requestAddProductVariantImage, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} started adding variant image for ProductVariantId {ProductVariantId}", vendorUserId, requestAddProductVariantImage.ProductVariantId);

        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var productVariant = await _productValidation.ValidateProductVariant(requestAddProductVariantImage.ProductVariantId, vendorUserId);
        await _productValidation.ValidateProductIfApproved(productVariant.ProductId);
        _logger.LogInformation("ProductVariant {ProductVariantId} validated. Parent ProductId {ProductId}", productVariant.ProductVariantId, productVariant.ProductId);
        var image = await _productImageRepsository.GetProductImageByImageURL(requestAddProductVariantImage.ImageUrl);
        if (image != null)
        {
            _logger.LogWarning("Duplicate image URL found while adding variant image for ProductVariantId {ProductVariantId}",
                requestAddProductVariantImage.ProductVariantId);

            throw new DataAlreadyRegisteredException("Image already added");
        }
        var productImage = _mapper.Map<ProductImage>(requestAddProductVariantImage);
        productImage.AddedByVendorUserId = vendorUser.VendorUserId;
        productImage.ProductId = productVariant.ProductId;
        await _productImageRepsository.Create(productImage);
        _logger.LogInformation("Product variant image {ProductImageId} added successfully for ProductVariantId {ProductVariantId}, ProductId {ProductId}", productImage.ProductImageId, productImage.ProductVariantId, productImage.ProductId);
        var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();

        _logger.LogInformation("Sending variant image notification to {AdminCount} product admins for ProductVariantId {ProductVariantId}", productAdminUserIds.Count, productVariant.ProductVariantId);

        foreach (var adminUserId in productAdminUserIds)
        {
            await _notificationService.SendToUser(
                adminUserId,
                "New Product Variant Image Added",
                $"A new image has been added for variant '{productVariant.SKU}' and may require review.",
                notificationTypeId: 1,
                referenceType: "ProductImage",
                referenceId: productImage.ProductImageId);
        }

        _logger.LogInformation("Product variant image notifications sent successfully for ProductImageId {ProductImageId}", productImage.ProductImageId);
        return _mapper.Map<ResponseAddProductVariantImage>(productImage);
    }
}