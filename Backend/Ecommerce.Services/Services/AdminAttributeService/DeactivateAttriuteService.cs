using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<ResponseAdminGetAttribute> DeactivateProductAttribute(int attributeId, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);
            _logger.LogInformation("Attribute deactivation initiated for AttributeId {AttributeId}", attributeId);

            var attribute = await _productAttributeValidation.ValidateAttribute(attributeId);

            attribute.IsActive = false;
            await _attributeRepsository.Update(attribute.AttributeMasterId, attribute);

            _logger.LogInformation("AttributeId {AttributeId} ({AttributeName}) deactivated successfully",
                attribute.AttributeMasterId, attribute.AttributeName);

            var logChanges = new LogChanges
            {
                TableName = nameof(AttributeMaster),
                RecordId = attribute.AttributeMasterId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"AttributeMasterId={attribute.AttributeMasterId}, AttributeName={attribute.AttributeName}, IsActive=True",
                NewValue = $"AttributeMasterId={attribute.AttributeMasterId}, AttributeName={attribute.AttributeName}, IsActive=False",
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
            _logger.LogInformation("Sending deactivation notification to {VendorCount} vendors", vendorOwnerIds.Count);
            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "Category Attribute Updated",
                    $"Attribute '{attribute.AttributeName}' has been deactivated. Please review your products.",
                    notificationTypeId: (int)NotificationTypeEnum.Attribute,
                    referenceType: "AttributeMaster",
                    referenceId: attribute.AttributeMasterId);
            }

            _logger.LogInformation("Attribute deactivation process completed for AttributeId {AttributeId}",attribute.AttributeMasterId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAdminGetAttribute>(attribute);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while deactivating AttributeId {AttributeId}", attributeId);

            await transaction.RollbackAsync();

            _logger.LogInformation("Transaction rolled back for AttributeId {AttributeId}", attributeId);

            throw;
        }
    }
    public async Task<ResponseAdminGetCategoryAttribute> DectivateProductSubCategoryAttribute(int subcategoryAttribute, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);

            _logger.LogInformation("ProductSubCategoryAttribute deactivation initiated for Id {ProductSubCategoryAttributeId}",
                subcategoryAttribute);

            var productSubCategory = await _productSubCategoryAttributeRepsository.Get(subcategoryAttribute);

            if (productSubCategory == null)
            {
                _logger.LogWarning("ProductSubCategoryAttribute not found for Id {ProductSubCategoryAttributeId}",
                    subcategoryAttribute);

                throw new DataNotFoundException("Product Sub Category Attribute is not found");
            }

            if (!productSubCategory.IsActive)
            {
                _logger.LogWarning("ProductSubCategoryAttributeId {ProductSubCategoryAttributeId} is already inactive",
                    productSubCategory.ProductSubCategoryAttributeId);

                throw new DataAlreadyRegisteredException("Product Sub category is already deactive");
            }

            productSubCategory.IsActive = false;

            await _productSubCategoryAttributeRepsository.Update(subcategoryAttribute, productSubCategory);

            _logger.LogInformation("ProductSubCategoryAttributeId {ProductSubCategoryAttributeId} deactivated successfully",
                productSubCategory.ProductSubCategoryAttributeId);

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductSubCategoryAttribute),
                RecordId = productSubCategory.ProductSubCategoryAttributeId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductSubCategoryAttributeId={productSubCategory.ProductSubCategoryAttributeId}, ProductSubCategoryId={productSubCategory.ProductSubCategoryId}, AttributeMasterId={productSubCategory.AttributeMasterId}, IsActive=True",
                NewValue = $"ProductSubCategoryAttributeId={productSubCategory.ProductSubCategoryAttributeId}, ProductSubCategoryId={productSubCategory.ProductSubCategoryId}, AttributeMasterId={productSubCategory.AttributeMasterId}, IsActive=False",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            _logger.LogInformation("Audit log created for ProductSubCategoryAttributeId {ProductSubCategoryAttributeId}",
                productSubCategory.ProductSubCategoryAttributeId);

            var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

            _logger.LogInformation("Sending subcategory attribute deactivation notification to {VendorCount} vendors",
                vendorOwnerIds.Count);

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "Category Attribute Updated",
                    $"A product subcategory attribute mapping for subcategory {productSubCategory.ProductSubCategoryId} has been deactivated. Please review your products.",
                    notificationTypeId: (int)NotificationTypeEnum.MappedAttribute,
                    referenceType: "ProductSubCategoryAttribute",
                    referenceId: productSubCategory.ProductSubCategoryAttributeId);
            }

            _logger.LogInformation("ProductSubCategoryAttribute deactivation process completed for Id {ProductSubCategoryAttributeId}",
                productSubCategory.ProductSubCategoryAttributeId);

            await transaction.CommitAsync();

            return _mapper.Map<ResponseAdminGetCategoryAttribute>(productSubCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Transaction failed while deactivating ProductSubCategoryAttributeId {ProductSubCategoryAttributeId}",
                subcategoryAttribute);

            await transaction.RollbackAsync();

            _logger.LogInformation("Transaction rolled back for ProductSubCategoryAttributeId {ProductSubCategoryAttributeId}",
                subcategoryAttribute);

            throw;
        }
    }
}