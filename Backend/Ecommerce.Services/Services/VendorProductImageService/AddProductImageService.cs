using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductImageService : IVendorProductImageService
{
    public async Task<ResponseAddProductImage> AddProductImage(RequestAddProductImage requestAddProductImage, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} started adding product image for ProductId {ProductId}", vendorUserId, requestAddProductImage.ProductId);

            var vendorUser = await _vendorUserValidation.ValidateProductVendorUserByUserId(vendorUserId);
            var product = await _productValidation.VendorValidateProduct(requestAddProductImage.ProductId, vendorUser.VendorId);
            _logger.LogInformation("Product {ProductId} validated for image upload", product.ProductId);

            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);

            var image = await _productImageRepsository.GetProductImageByImageURL(requestAddProductImage.ImageUrl);
            if (image != null)
            {
                _logger.LogWarning("Duplicate image URL found while adding product image for ProductId {ProductId}", requestAddProductImage.ProductId);
                throw new DataAlreadyRegisteredException("Image already added");
            }

            var productImage = _mapper.Map<ProductImage>(requestAddProductImage);
            if (productImage == null)
            {
                _logger.LogError("Failed to map RequestAddProductImage to ProductImage entity for ProductId {ProductId}", requestAddProductImage.ProductId);
                throw new NullReferenceException("Product image mapping failed");
            }
            productImage.AddedByVendorUserId = vendorUser.VendorUserId;

            var createdImage = await _productImageRepsository.Create(productImage);
            if (createdImage == null)
            {
                _logger.LogError("Failed to create ProductImage for ProductId {ProductId}", requestAddProductImage.ProductId);
                throw new DataRegistrationException("Product image creation failed");
            }
            _logger.LogInformation("Product image {ProductImageId} added successfully for ProductId {ProductId}", createdImage.ProductImageId, createdImage.ProductId);

            var imageLog = new LogChanges
            {
                TableName = nameof(ProductImage),
                RecordId = createdImage.ProductImageId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"ProductImageId={createdImage.ProductImageId}, ProductId={createdImage.ProductId}, ImageUrl={createdImage.ImageUrl}, IsMainImage={createdImage.IsMainImage}",
                UserId = vendorUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(imageLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", imageLog.TableName, imageLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", imageLog.TableName, imageLog.RecordId);

            if (requestAddProductImage.IsMainImage == true)
            {
                _logger.LogInformation("Setting ProductImageId {ProductImageId} as default image for ProductId {ProductId}", createdImage.ProductImageId, createdImage.ProductId);
                RequestMakeDefaultImageDTO requestMakeDefaultImageDTO = new RequestMakeDefaultImageDTO();
                requestMakeDefaultImageDTO.ProductImageId = createdImage.ProductImageId;
                await MakeImageDefault(requestMakeDefaultImageDTO,vendorUserId);
            }

            var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();
            foreach (var adminUserId in productAdminUserIds)
            {
                await _notificationService.SendToUser(
                    adminUserId,
                    "New Product Image Added",
                    $"A new image has been added for product '{product.ProductName}' and may require review.",
                    notificationTypeId: (int)NotificationTypeEnum.ProductImageAdded,
                    referenceType: "ProductImage",
                    referenceId: createdImage.ProductImageId);
            }
            await transaction.CommitAsync();
            _logger.LogInformation("Product image notifications sent successfully for ProductImageId {ProductImageId}", createdImage.ProductImageId);
            return _mapper.Map<ResponseAddProductImage>(createdImage);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while adding product image for ProductId {ProductId}", requestAddProductImage.ProductId);
            _logger.LogInformation("Transaction rolled back while adding product image for ProductId {ProductId}", requestAddProductImage.ProductId);
            throw;
        }
    }

    public async Task<ResponseMakeDefaultImageDTO> MakeImageDefault(RequestMakeDefaultImageDTO requestMakeDefaultImageDTO,int userId)
    {
        _logger.LogInformation("Setting ProductImageId: {ProductImageId} as the default image", requestMakeDefaultImageDTO.ProductImageId);

        var image = await _productValidation.ValidateProductImage(requestMakeDefaultImageDTO.ProductImageId);

        var productImageList = await _productImageRepsository.GetAllProductImageByProductId(image.ProductId);
        _logger.LogInformation("Found {ImageCount} images for ProductId: {ProductId}", productImageList.Count, image.ProductId);

        foreach (var images in productImageList)
        {
            if (images.ProductImageId == image.ProductImageId)
            {
                // this is the image being made default; handled separately below
                continue;
            }

            if (images.IsMainImage != true)
            {
                // no actual change, skip logging/updating a no-op
                continue;
            }

            bool previousIsMainImage = images.IsMainImage;
            images.IsMainImage = false;
            images.UpdatedAt = DateTime.Now;

            var updatedImage = await _productImageRepsository.Update(images.ProductImageId, images);
            if (updatedImage == null)
            {
                _logger.LogError("Failed to clear default flag on ProductImageId {ProductImageId}", images.ProductImageId);
                throw new DataRegistrationException("Updation of the product image failed");
            }

            var clearLog = new LogChanges
            {
                TableName = nameof(ProductImage),
                RecordId = updatedImage.ProductImageId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductImageId={images.ProductImageId}, IsMainImage={previousIsMainImage}",
                NewValue = $"ProductImageId={updatedImage.ProductImageId}, IsMainImage={updatedImage.IsMainImage}",
                ChangedAt = DateTime.Now
            };
            var createdClearLog = await _logChanges.Create(clearLog);
            if (createdClearLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", clearLog.TableName, clearLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
        }

        _logger.LogInformation("Cleared existing default image for ProductId: {ProductId}", image.ProductId);

        bool previousImageIsMainImage = image.IsMainImage;
        image.IsMainImage = true;
        image.UpdatedAt = DateTime.Now;

        var updatedDefaultImage = await _productImageRepsository.Update(image.ProductImageId, image);
        if (updatedDefaultImage == null)
        {
            _logger.LogError("Failed to set ProductImageId {ProductImageId} as default", image.ProductImageId);
            throw new DataRegistrationException("Updation of the product image failed");
        }
        _logger.LogInformation("Successfully set ProductImageId: {ProductImageId} as the default image for ProductId: {ProductId}", updatedDefaultImage.ProductImageId, updatedDefaultImage.ProductId);

        var defaultLog = new LogChanges
        {
            TableName = nameof(ProductImage),
            RecordId = updatedDefaultImage.ProductImageId,
            Actions = (int)AuditAction.Updated,
            OldValue = $"ProductImageId={image.ProductImageId}, IsMainImage={previousImageIsMainImage}",
            NewValue = $"ProductImageId={updatedDefaultImage.ProductImageId}, IsMainImage={updatedDefaultImage.IsMainImage}",
            ChangedAt = DateTime.Now,
            UserId = userId
        };
        var createdDefaultLog = await _logChanges.Create(defaultLog);
        if (createdDefaultLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", defaultLog.TableName, defaultLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", defaultLog.TableName, defaultLog.RecordId);

        return _mapper.Map<ResponseMakeDefaultImageDTO>(updatedDefaultImage);
    }

    public async Task<ResponseAddProductVariantImage> AddProductVariantImage(RequestAddProductVariantImage requestAddProductVariantImage, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} started adding variant image for ProductVariantId {ProductVariantId}", vendorUserId, requestAddProductVariantImage.ProductVariantId);

            var vendorUser = await _vendorUserValidation.ValidateProductVendorUserByUserId(vendorUserId);
            var productVariant = await _productValidation.ValidateProductVariant(requestAddProductVariantImage.ProductVariantId, vendorUserId);
            var vendor = await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
            await _productValidation.ValidateProductIfApproved(productVariant.ProductId);
            _logger.LogInformation("ProductVariant {ProductVariantId} validated. Parent ProductId {ProductId}", productVariant.ProductVariantId, productVariant.ProductId);

            var image = await _productImageRepsository.GetProductImageByImageURL(requestAddProductVariantImage.ImageUrl);
            if (image != null)
            {
                _logger.LogWarning("Duplicate image URL found while adding variant image for ProductVariantId {ProductVariantId}", requestAddProductVariantImage.ProductVariantId);
                throw new DataAlreadyRegisteredException("Image already added");
            }

            var productImage = _mapper.Map<ProductImage>(requestAddProductVariantImage);
            if (productImage == null)
            {
                _logger.LogError("Failed to map RequestAddProductVariantImage to ProductImage entity for ProductVariantId {ProductVariantId}", requestAddProductVariantImage.ProductVariantId);
                throw new NullReferenceException("Product variant image mapping failed");
            }
            productImage.AddedByVendorUserId = vendorUser.VendorUserId;
            productImage.ProductId = productVariant.ProductId;

            var createdImage = await _productImageRepsository.Create(productImage);
            if (createdImage == null)
            {
                _logger.LogError("Failed to create ProductImage for ProductVariantId {ProductVariantId}", requestAddProductVariantImage.ProductVariantId);
                throw new DataRegistrationException("Product variant image creation failed");
            }
            _logger.LogInformation("Product variant image {ProductImageId} added successfully for ProductVariantId {ProductVariantId}, ProductId {ProductId}", createdImage.ProductImageId, createdImage.ProductVariantId, createdImage.ProductId);

            var imageLog = new LogChanges
            {
                TableName = nameof(ProductImage),
                RecordId = createdImage.ProductImageId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"ProductImageId={createdImage.ProductImageId}, ProductVariantId={createdImage.ProductVariantId}, ProductId={createdImage.ProductId}, ImageUrl={createdImage.ImageUrl}",
                UserId = vendorUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(imageLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", imageLog.TableName, imageLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", imageLog.TableName, imageLog.RecordId);

            var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();
            _logger.LogInformation("Sending variant image notification to {AdminCount} product admins for ProductVariantId {ProductVariantId}", productAdminUserIds.Count, productVariant.ProductVariantId);

            foreach (var adminUserId in productAdminUserIds)
            {
                await _notificationService.SendToUser(
                    adminUserId,
                    "New Product Variant Image Added",
                    $"A new image has been added for variant '{productVariant.SKU}' and may require review.",
                    notificationTypeId: (int)NotificationTypeEnum.ProductImageAdded,
                    referenceType: "ProductImage",
                    referenceId: createdImage.ProductImageId);
            }
            await transaction.CommitAsync();
            _logger.LogInformation("Product variant image notifications sent successfully for ProductImageId {ProductImageId}", createdImage.ProductImageId);
            return _mapper.Map<ResponseAddProductVariantImage>(createdImage);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while adding product variant image for ProductVariantId {ProductVariantId}", requestAddProductVariantImage.ProductVariantId);
            _logger.LogInformation("Transaction rolled back while adding product variant image for ProductVariantId {ProductVariantId}", requestAddProductVariantImage.ProductVariantId);
            throw;
        }
    }
}