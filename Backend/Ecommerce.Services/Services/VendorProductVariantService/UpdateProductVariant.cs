using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    public async Task<ResponseUpdateProductVariantDTO> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO, int vendoruserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} updating ProductVariantId {ProductVariantId}", vendoruserId, requestUpdateProductVariantDTO.ProductVariantId);

            var vendorUser = await _vendorUserValidation.ValidateProductVendorUserByUserId(vendoruserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
            _logger.LogInformation("Vendor {VendorId} is approved and eligible to update product variants", vendorUser.VendorId);

            var productVariant = await _productValidation.ValidateProductVariant(requestUpdateProductVariantDTO.ProductVariantId, vendoruserId);
            _logger.LogInformation("ProductVariantId {ProductVariantId} validated successfully for status update", productVariant.ProductVariantId);

            int previousStatusId = productVariant.ProductVariantStatusId;
            productVariant.UpdatedAt = DateTime.Now;
            productVariant.ProductVariantStatusId = requestUpdateProductVariantDTO.ProductStatusId;
            _logger.LogInformation("Updating ProductVariantId {ProductVariantId} status from {OldStatus} to {NewStatus}", productVariant.ProductVariantId, previousStatusId, productVariant.ProductVariantStatusId);

            var updatedVariant = await _productVariantRepsository.Update(productVariant.ProductVariantId, productVariant);
            if (updatedVariant == null)
            {
                _logger.LogError("Failed to update ProductVariantId {ProductVariantId}", productVariant.ProductVariantId);
                throw new DataRegistrationException("Updation of the product variant failed");
            }
            _logger.LogInformation("ProductVariantId {ProductVariantId} updated successfully by Vendor UserId {VendorUserId}", updatedVariant.ProductVariantId, vendoruserId);

            var variantLog = new LogChanges
            {
                TableName = nameof(ProductVariant),
                RecordId = updatedVariant.ProductVariantId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductVariantId={productVariant.ProductVariantId}, ProductVariantStatusId={previousStatusId}",
                NewValue = $"ProductVariantId={updatedVariant.ProductVariantId}, ProductVariantStatusId={updatedVariant.ProductVariantStatusId}",
                UserId = vendoruserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(variantLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", variantLog.TableName, variantLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", variantLog.TableName, variantLog.RecordId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseUpdateProductVariantDTO>(updatedVariant);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while updating ProductVariantId {ProductVariantId}, VendorUserId {VendorUserId}", requestUpdateProductVariantDTO.ProductVariantId, vendoruserId);
            throw;
        }
    }

    public async Task<ResponseUpdateProductVariantDTO> UpdateRejectedProductVariant(RequestUpdateProductVariant requestUpdateProductVariantDTO, int vendoruserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} updating rejected ProductVariantId {ProductVariantId}", vendoruserId, requestUpdateProductVariantDTO.ProductVariantId);

            var vendorUser = await _vendorUserValidation.ValidateOwnerVendorUserByUserId(vendoruserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
            _logger.LogInformation("Vendor {VendorId} is approved and eligible to update product variants", vendorUser.VendorId);

            var productVariant = await _productValidation.ValidateProductVariant(requestUpdateProductVariantDTO.ProductVariantId, vendoruserId);
            _logger.LogInformation("Current ProductVariant Approval Status: {ApprovalStatusId} for ProductVariantId {ProductVariantId}", productVariant.ProductApprovalStatusId, productVariant.ProductVariantId);

            if (productVariant.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved || productVariant.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Deleted_By_Admin)
            {
                _logger.LogWarning("ProductVariantId {ProductVariantId} cannot be updated because ApprovalStatusId is {ApprovalStatusId}", productVariant.ProductVariantId, productVariant.ProductApprovalStatusId);
                throw new DataApprovalStatusException("Product Variant Cannot be updated if admin approved");
            }

            int previousApprovalStatusId = productVariant.ProductApprovalStatusId;

            _mapper.Map(requestUpdateProductVariantDTO, productVariant);
            productVariant.UpdatedAt = DateTime.Now;

            productVariant.ProductApprovalStatusId = (int)ProductApprovalStatusEnum.Vendor_Approved;
            _logger.LogInformation("Updating ProductVariantId {ProductVariantId}. Variant will be resubmitted for admin review", productVariant.ProductVariantId);

            var updatedVariant = await _productVariantRepsository.Update(productVariant.ProductVariantId, productVariant);
            if (updatedVariant == null)
            {
                _logger.LogError("Failed to update ProductVariantId {ProductVariantId}", productVariant.ProductVariantId);
                throw new DataRegistrationException("Updation of the product variant failed");
            }
            productVariant = updatedVariant;
            _logger.LogInformation("Rejected ProductVariantId {ProductVariantId} updated successfully by Vendor UserId {VendorUserId}", productVariant.ProductVariantId, vendoruserId);

            var variantLog = new LogChanges
            {
                TableName = nameof(ProductVariant),
                RecordId = productVariant.ProductVariantId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductVariantId={productVariant.ProductVariantId}, ProductApprovalStatusId={previousApprovalStatusId}",
                NewValue = $"ProductVariantId={productVariant.ProductVariantId}, ProductApprovalStatusId={productVariant.ProductApprovalStatusId}",
                UserId = vendoruserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(variantLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", variantLog.TableName, variantLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", variantLog.TableName, variantLog.RecordId);

            var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();
            _logger.LogInformation("Sending product variant update notification to {AdminCount} product admins for ProductVariantId {ProductVariantId}", productAdminUserIds.Count, productVariant.ProductVariantId);

            if (productAdminUserIds.Count == 0)
            {
                _logger.LogWarning("No product admin users found to notify for ProductVariantId {ProductVariantId}", productVariant.ProductVariantId);
            }

            foreach (var adminUserId in productAdminUserIds)
            {
                await _notificationService.SendToUser(
                    adminUserId,
                    "Product Variant Updated By Vendor",
                    $"Product variant '{productVariant.SKU}' has been modified by the vendor and requires review.",
                    notificationTypeId: (int)NotificationTypeEnum.ProductSubmitted,
                    referenceType: "ProductVariant",
                    referenceId: productVariant.ProductVariantId);
                _logger.LogInformation("Product variant update notification sent to product admin UserId {UserId}", adminUserId);
            }

            _logger.LogInformation("Product variant review notifications sent successfully for ProductVariantId {ProductVariantId}", productVariant.ProductVariantId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseUpdateProductVariantDTO>(productVariant);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while updating rejected ProductVariantId {ProductVariantId}, VendorUserId {VendorUserId}", requestUpdateProductVariantDTO.ProductVariantId, vendoruserId);
            throw;
        }
    }
}