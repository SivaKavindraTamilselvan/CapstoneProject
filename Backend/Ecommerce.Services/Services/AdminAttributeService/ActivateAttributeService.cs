using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<ResponseAdminGetAttribute> ActivateProductAttribute(int attributeId, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);
            _logger.LogInformation("Attribute activation initiated for AttributeId {AttributeId}", attributeId);
            var attribute = await _attributeRepsository.Get(attributeId);
            if (attribute == null)
            {
                _logger.LogWarning("Attribute not found for AttributeId {AttributeId}", attributeId);
                throw new DataNotFoundException("Product Category is not found");
            }
            if (attribute.IsActive)
            {
                _logger.LogWarning("AttributeId {AttributeId} is already active", attributeId);
                throw new DataAlreadyRegisteredException("Product category is already active");
            }
            attribute.IsActive = true;
            var updatedAttribute = await _attributeRepsository.Update(attribute.AttributeMasterId, attribute);
            if (updatedAttribute == null)
            {
                _logger.LogError("Failed to update AttributeId {AttributeId} by AdminUserId {AdminUserId}", attributeId, adminUserId);
                throw new DataRegistrationException("Attribute update failed.");
            }
            _logger.LogInformation("AttributeId {AttributeId} activated successfully", updatedAttribute.AttributeMasterId);

            var logChanges = new LogChanges
            {
                TableName = nameof(AttributeMaster),
                RecordId = updatedAttribute.AttributeMasterId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"AttributeMasterId={attribute.AttributeMasterId}, AttributeName={attribute.AttributeName}, IsActive=False",
                NewValue = $"AttributeMasterId={updatedAttribute.AttributeMasterId}, AttributeName={updatedAttribute.AttributeName}, IsActive=True",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            _logger.LogInformation("Audit log created for AttributeId {AttributeId}", attribute.AttributeMasterId);

            var vendorOwnerIds = await _vendorUserRepsository.GetAllProductVendorUserIds();
            _logger.LogInformation("Sending attribute activation notification to {VendorCount} vendors", vendorOwnerIds.Count);

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "Category Attribute Updated",
                    $"Attribute '{updatedAttribute.AttributeName}' has been activated. Please review your products.",
                    notificationTypeId: (int)NotificationTypeEnum.Attribute,
                    referenceType: "AttributeMaster",
                    referenceId: updatedAttribute.AttributeMasterId);
            }

            _logger.LogInformation("Attribute activation process completed for AttributeId {AttributeId}", attribute.AttributeMasterId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAdminGetAttribute>(updatedAttribute);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while activating AttributeId {AttributeId}", attributeId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for AttributeId {AttributeId}", attributeId);
            throw;
        }
    }

    public async Task<ResponseAdminGetCategoryAttribute> ActivateProductSubCategoryAttribute(int subcategoryAttribute, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);
            _logger.LogInformation("ProductSubCategoryAttribute activation initiated for Id {ProductSubCategoryAttributeId}", subcategoryAttribute);

            var productSubCategory = await _productSubCategoryAttributeRepsository.Get(subcategoryAttribute);
            if (productSubCategory == null)
            {
                _logger.LogWarning("ProductSubCategoryAttribute not found for Id {ProductSubCategoryAttributeId}", subcategoryAttribute);
                throw new DataNotFoundException("Product Sub Category Attribute is not found");
            }
            if (productSubCategory.IsActive)
            {
                _logger.LogWarning("ProductSubCategoryAttributeId {ProductSubCategoryAttributeId} is already active", productSubCategory.ProductSubCategoryAttributeId);
                throw new DataAlreadyRegisteredException("Product Sub category is already active");
            }
            productSubCategory.IsActive = true;
            var updatedProductSubCategoryAttribute = await _productSubCategoryAttributeRepsository.Update(subcategoryAttribute, productSubCategory);

            if (updatedProductSubCategoryAttribute == null)
            {
                _logger.LogError("Failed to update ProductSubCategoryAttributeId {ProductSubCategoryAttributeId} by AdminUserId {AdminUserId}", subcategoryAttribute, adminUserId);
                throw new DataRegistrationException("Product SubCategory Attribute update failed.");
            }

            _logger.LogInformation("ProductSubCategoryAttributeId {ProductSubCategoryAttributeId} activated successfully",
            productSubCategory.ProductSubCategoryAttributeId);

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductSubCategoryAttribute),
                RecordId = updatedProductSubCategoryAttribute.ProductSubCategoryAttributeId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductSubCategoryAttributeId={productSubCategory.ProductSubCategoryAttributeId}, ProductSubCategoryId={productSubCategory.ProductSubCategoryId}, AttributeMasterId={productSubCategory.AttributeMasterId}, IsActive=False",
                NewValue = $"ProductSubCategoryAttributeId={updatedProductSubCategoryAttribute.ProductSubCategoryAttributeId}, ProductSubCategoryId={updatedProductSubCategoryAttribute.ProductSubCategoryId}, AttributeMasterId={updatedProductSubCategoryAttribute.AttributeMasterId}, IsActive=True",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            _logger.LogInformation("Audit log created for ProductSubCategoryAttributeId {ProductSubCategoryAttributeId}", productSubCategory.ProductSubCategoryAttributeId);
            var vendorOwnerIds = await _vendorUserRepsository.GetAllProductVendorUserIds();
            _logger.LogInformation("Sending ProductSubCategoryAttribute activation notification to {VendorCount} vendors", vendorOwnerIds.Count);

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "Category Attribute Updated",
                    $"A product subcategory attribute mapping has been activated for subcategory {updatedProductSubCategoryAttribute.ProductSubCategoryId}. Please review your products.",
                    notificationTypeId: (int)NotificationTypeEnum.MappedAttribute,
                    referenceType: "ProductSubCategoryAttribute",
                    referenceId: updatedProductSubCategoryAttribute.ProductSubCategoryAttributeId);
            }

            _logger.LogInformation("ProductSubCategoryAttribute activation process completed for Id {ProductSubCategoryAttributeId}", productSubCategory.ProductSubCategoryAttributeId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAdminGetCategoryAttribute>(updatedProductSubCategoryAttribute);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while activating ProductSubCategoryAttributeId {ProductSubCategoryAttributeId}", subcategoryAttribute);

            await transaction.RollbackAsync();

            _logger.LogInformation("Transaction rolled back for ProductSubCategoryAttributeId {ProductSubCategoryAttributeId}", subcategoryAttribute);

            throw;
        }
    }
}