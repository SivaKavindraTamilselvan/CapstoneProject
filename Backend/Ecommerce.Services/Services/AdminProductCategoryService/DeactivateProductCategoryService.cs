using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseAdminGetAllCategory> DeactivateProductCategory(int productCategoryId, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);

            _logger.LogInformation("Deactivating Product Category {CategoryId}", productCategoryId);

            var productCategory = await _productCategoryValidation.ValidateCategory(productCategoryId);

            productCategory.IsActive = false;
            var updatedCategory = await _productCategoryRepsository.Update(productCategoryId, productCategory);
            if (updatedCategory == null)
            {
                _logger.LogError("Failed to update ProductCategoryId {ProductCategoryId} by AdminUserId {AdminUserId}", productCategoryId, adminUserId);
                throw new DataRegistrationException("Product Category update failed.");
            }

            _logger.LogInformation("Product Category {CategoryId} - {CategoryName} deactivated successfully",
                productCategory.ProductCategoryId,
                productCategory.ProductCategoryName);

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductCategory),
                RecordId = productCategory.ProductCategoryId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductCategoryId={productCategory.ProductCategoryId}, ProductCategoryName={productCategory.ProductCategoryName}, IsActive=True",
                NewValue = $"ProductCategoryId={updatedCategory.ProductCategoryId}, ProductCategoryName={updatedCategory.ProductCategoryName}, IsActive=False",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            _logger.LogInformation("Audit log created for ProductCategoryId {CategoryId}", productCategory.ProductCategoryId);

            var vendorOwnerIds = await _vendorUserRepsository.GetAllProductVendorUserIds();

            _logger.LogInformation("Sending category deactivation notification to {VendorCount} vendors", vendorOwnerIds.Count);

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "Category Deactivated",
                    $"Product category '{updatedCategory.ProductCategoryName}' has been deactivated and is now not available for use.",
                    notificationTypeId: (int)NotificationTypeEnum.Category,
                    referenceType: "ProductCategory",
                    referenceId: updatedCategory.ProductCategoryId);
            }
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAdminGetAllCategory>(updatedCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while deactivating ProductCategoryId {CategoryId}", productCategoryId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for ProductCategoryId {CategoryId}", productCategoryId);
            throw;
        }
    }
    public async Task<ResponseAdminGetAllSubCategory> DeactivateProductSubCategory(int productSubCategoryId, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);

            _logger.LogInformation("Deactivating Product SubCategory {SubCategoryId}", productSubCategoryId);

            var productSubCategory = await _productCategoryValidation.ValidateSubCategory(productSubCategoryId);

            productSubCategory.IsActive = false;
            var updatedSubCategory = await _productSubCategoryRepsository.Update(productSubCategoryId, productSubCategory);
            if (updatedSubCategory == null)
            {
                _logger.LogError("Failed to update ProductSubCategoryId {ProductSubCategoryId} by AdminUserId {AdminUserId}", productSubCategoryId, adminUserId);
                throw new DataRegistrationException("Product SubCategory update failed.");
            }

            _logger.LogInformation("Product SubCategory {SubCategoryId} - {SubCategoryName} deactivated successfully",
                productSubCategory.ProductSubCategoryId,
                productSubCategory.ProductSubCategoryName);

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductSubCategory),
                RecordId = productSubCategory.ProductSubCategoryId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductSubCategoryId={productSubCategory.ProductSubCategoryId}, ProductSubCategoryName={productSubCategory.ProductSubCategoryName}, ProductCategoryId={productSubCategory.ProductCategoryId}, IsActive=True",
                NewValue = $"ProductSubCategoryId={updatedSubCategory.ProductSubCategoryId}, ProductSubCategoryName={updatedSubCategory.ProductSubCategoryName}, ProductCategoryId={productSubCategory.ProductCategoryId}, IsActive=False",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for ProductSubCategoryId {SubCategoryId}", productSubCategory.ProductSubCategoryId);

            var vendorOwnerIds = await _vendorUserRepsository.GetAllProductVendorUserIds();

            _logger.LogInformation("Sending subcategory deactivation notification to {VendorCount} vendors", vendorOwnerIds.Count);

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "SubCategory Deactivated",
                    $"Product subcategory '{updatedSubCategory.ProductSubCategoryName}' has been deactivated and is now not available for use.",
                    notificationTypeId: (int)NotificationTypeEnum.SubCategory,
                    referenceType: "ProductSubCategory",
                    referenceId: updatedSubCategory.ProductSubCategoryId);
            }
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAdminGetAllSubCategory>(updatedSubCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while deactivating ProductSubCategoryId {SubCategoryId}", productSubCategoryId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for ProductSubCategoryId {SubCategoryId}", productSubCategoryId);
            throw;
        }
    }
}