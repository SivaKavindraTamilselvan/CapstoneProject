using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseAdminGetAllCategory> ActivateProductCategory(int productCategoryId, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);
            _logger.LogInformation("Activating Product Category {CategoryId}", productCategoryId);

            var productCategory = await _productCategoryRepsository.Get(productCategoryId);

            if (productCategory == null)
            {
                _logger.LogWarning("Product Category {CategoryId} not found", productCategoryId);
                throw new DataNotFoundException("Product Category is not found");
            }

            if (productCategory.IsActive)
            {
                _logger.LogWarning("Product Category {CategoryId} is already active", productCategoryId);
                throw new DataAlreadyRegisteredException("Product category is already active");
            }

            productCategory.IsActive = true;

            var updatedCategory = await _productCategoryRepsository.Update(productCategoryId, productCategory);
            if (updatedCategory == null)
            {
                _logger.LogError("Failed to update ProductCategoryId {ProductCategoryId} by AdminUserId {AdminUserId}", productCategoryId, adminUserId);
                throw new DataRegistrationException("Product Category update failed.");
            }
            _logger.LogInformation("Product Category {CategoryId} - {CategoryName} activated successfully", updatedCategory.ProductCategoryId, updatedCategory.ProductCategoryName);

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductCategory),
                RecordId = updatedCategory.ProductCategoryId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductCategoryId={productCategory.ProductCategoryId}, ProductCategoryName={productCategory.ProductCategoryName}, IsActive=False",
                NewValue = $"ProductCategoryId={updatedCategory.ProductCategoryId}, ProductCategoryName={updatedCategory.ProductCategoryName}, IsActive=True,AdminUserId={adminUserId}",
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
            _logger.LogInformation("Sending category activation notification to {VendorCount} vendors", vendorOwnerIds.Count);


            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "Category Activated",
                    $"Product category '{updatedCategory.ProductCategoryName}' has been activated and is now available for use.",
                    notificationTypeId: (int)NotificationTypeEnum.Category,
                    referenceType: "ProductCategory",
                    referenceId: updatedCategory.ProductCategoryId);
            }
            await transaction.CommitAsync();
            
            return _mapper.Map<ResponseAdminGetAllCategory>(updatedCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while activating ProductCategoryId {CategoryId}", productCategoryId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for ProductCategoryId {CategoryId}", productCategoryId);
            throw;
        }
    }
    public async Task<ResponseAdminGetAllSubCategory> ActivateProductSubCategory(int productSubCategoryId, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);
            _logger.LogInformation("Activating Product SubCategory {SubCategoryId}", productSubCategoryId);

            var productSubCategory = await _productSubCategoryRepsository.Get(productSubCategoryId);

            if (productSubCategory == null)
            {
                _logger.LogWarning("Product SubCategory {SubCategoryId} not found", productSubCategoryId);
                throw new DataNotFoundException("Product Sub Category is not found");
            }

            if (productSubCategory.IsActive)
            {
                _logger.LogWarning("Product SubCategory {SubCategoryId} is already active", productSubCategoryId);
                throw new DataAlreadyRegisteredException("Product Sub category is already active");
            }

            productSubCategory.IsActive = true;
            var updatedSubCategory = await _productSubCategoryRepsository.Update(productSubCategoryId, productSubCategory);
            if (updatedSubCategory == null)
            {
                _logger.LogError("Failed to update ProductSubCategoryId {ProductSubCategoryId} by AdminUserId {AdminUserId}", productSubCategoryId, adminUserId);

                throw new DataRegistrationException("Product SubCategory update failed.");
            }
            _logger.LogInformation("Product SubCategory {SubCategoryId} - {SubCategoryName} activated successfully", updatedSubCategory.ProductSubCategoryId, updatedSubCategory.ProductSubCategoryName);

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductSubCategory),
                RecordId = productSubCategory.ProductSubCategoryId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductSubCategoryId={productSubCategory.ProductSubCategoryId}, ProductSubCategoryName={productSubCategory.ProductSubCategoryName}, ProductCategoryId={productSubCategory.ProductCategoryId}, IsActive=False",
                NewValue = $"ProductSubCategoryId={updatedSubCategory.ProductSubCategoryId}, ProductSubCategoryName={updatedSubCategory.ProductSubCategoryName}, ProductCategoryId={updatedSubCategory.ProductCategoryId}, IsActive=True",
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
            _logger.LogInformation("Sending subcategory activation notification to {VendorCount} vendors", vendorOwnerIds.Count);

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "SubCategory Activated",
                    $"Product subcategory '{updatedSubCategory.ProductSubCategoryName}' has been activated and is now available for use.",
                    notificationTypeId: (int)NotificationTypeEnum.SubCategory,
                    referenceType: "ProductSubCategory",
                    referenceId: updatedSubCategory.ProductSubCategoryId);
            }
            await transaction.CommitAsync();
            
            return _mapper.Map<ResponseAdminGetAllSubCategory>(updatedSubCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while activating ProductSubCategoryId {SubCategoryId}", productSubCategoryId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for ProductSubCategoryId {SubCategoryId}", productSubCategoryId);
            throw;
        }
    }
}