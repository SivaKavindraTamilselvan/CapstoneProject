using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductImageService : IVendorProductImageService
{
    public async Task<ResponseMakeDefaultImageDTO> DeleteProductImage(int productImageId, int userid)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} requested deletion of ProductImageId {ProductImageId}", userid, productImageId);

            var product = await _productImageRepsository.Get(productImageId);
            if (product == null)
            {
                _logger.LogWarning("ProductImageId {ProductImageId} not found", productImageId);
                throw new DataNotFoundException("Product Image Is Not Found");
            }

            var vendorUser = await _vendorUserValidation.ValidateProductVendorUserByUserId(userid);
            await _productValidation.VendorValidateProduct(product.ProductId, vendorUser.VendorId);
            _logger.LogInformation("Product {ProductId} validated for image deletion", product.ProductId);

            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);

            if (product.IsMainImage)
            {
                _logger.LogWarning("Vendor UserId {VendorUserId} attempted to delete main image ProductImageId {ProductImageId}", userid, productImageId);
                throw new DataApprovalStatusException("Main Image Cannot Be Deleted");
            }

            var deleted = await _productImageRepsository.Delete(productImageId);
            if (deleted == null)
            {
                _logger.LogError("Failed to delete ProductImageId {ProductImageId}", productImageId);
                throw new DataRegistrationException("Deletion of the product image failed");
            }
            _logger.LogInformation("ProductImageId {ProductImageId} deleted successfully by Vendor UserId {VendorUserId}", productImageId, userid);

            var imageLog = new LogChanges
            {
                TableName = nameof(ProductImage),
                RecordId = product.ProductImageId,
                Actions = (int)AuditAction.Deleted,
                OldValue = $"ProductImageId={product.ProductImageId}, ProductId={product.ProductId}, ImageUrl={product.ImageUrl}, IsMainImage={product.IsMainImage}",
                NewValue = string.Empty,
                UserId = userid,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(imageLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", imageLog.TableName, imageLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", imageLog.TableName, imageLog.RecordId);

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for ProductImageId {ProductImageId}", productImageId);

            return _mapper.Map<ResponseMakeDefaultImageDTO>(product);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while deleting ProductImageId {ProductImageId}", productImageId);
            _logger.LogInformation("Transaction rolled back while deleting ProductImageId {ProductImageId}", productImageId);
            throw;
        }
    }
}