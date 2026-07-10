using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<ResponseAdminGetAttribute> ActivateProductAttribute(int attributeId,int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Attribute activation initiated for AttributeId {AttributeId}",attributeId);
        var attribute = await _attributeRepsository.Get(attributeId);
        if (attribute == null)
        {
            _logger.LogWarning("Attribute not found for AttributeId {AttributeId}",attributeId);
            throw new DataNotFoundException("Product Category is not found");
        }
        if (attribute.IsActive)
        {
            _logger.LogWarning("AttributeId {AttributeId} is already active",attributeId);
            throw new DataAlreadyRegisteredException("Product category is already active");
        }
        attribute.IsActive = true;
        await _attributeRepsository.Update(attribute.AttributeMasterId, attribute);

        _logger.LogInformation("AttributeId {AttributeId} activated successfully",attribute.AttributeMasterId);
        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

        _logger.LogInformation("Sending attribute activation notification to {VendorCount} vendors",vendorOwnerIds.Count);

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "Category Attribute Updated",
                $"Attribute '{attribute.AttributeName}' has been activated. Please review your products.",
                notificationTypeId: 1,
                referenceType: "AttributeMaster",
                referenceId: attribute.AttributeMasterId);
        }

        _logger.LogInformation("Attribute activation process completed for AttributeId {AttributeId}",attribute.AttributeMasterId);
        return _mapper.Map<ResponseAdminGetAttribute>(attribute);
    }
    public async Task<ResponseAdminGetCategoryAttribute> ActivateProductSubCategoryAttribute(int subcategoryAttribute,int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("ProductSubCategoryAttribute activation initiated for Id {ProductSubCategoryAttributeId}",subcategoryAttribute);

        var productSubCategory = await _productSubCategoryAttributeRepsository.Get(subcategoryAttribute);
        if (productSubCategory == null)
        {
            _logger.LogWarning("ProductSubCategoryAttribute not found for Id {ProductSubCategoryAttributeId}",subcategoryAttribute);
            throw new DataNotFoundException("Product Sub Category Attribute is not found");
        }
        if (productSubCategory.IsActive)
        {
            _logger.LogWarning("ProductSubCategoryAttributeId {ProductSubCategoryAttributeId} is already active",productSubCategory.ProductSubCategoryAttributeId);
            throw new DataAlreadyRegisteredException("Product Sub category is already active");
        }
        productSubCategory.IsActive = true;
        await _productSubCategoryAttributeRepsository.Update(subcategoryAttribute, productSubCategory);
        
        _logger.LogInformation("ProductSubCategoryAttributeId {ProductSubCategoryAttributeId} activated successfully",
        productSubCategory.ProductSubCategoryAttributeId);

        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

        _logger.LogInformation("Sending ProductSubCategoryAttribute activation notification to {VendorCount} vendors",vendorOwnerIds.Count);

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "Category Attribute Updated",
                $"A product subcategory attribute mapping has been activated for subcategory {productSubCategory.ProductSubCategoryId}. Please review your products.",
                notificationTypeId: 1,
                referenceType: "ProductSubCategoryAttribute",
                referenceId: productSubCategory.ProductSubCategoryAttributeId);
        }

        _logger.LogInformation("ProductSubCategoryAttribute activation process completed for Id {ProductSubCategoryAttributeId}",productSubCategory.ProductSubCategoryAttributeId);
        return _mapper.Map<ResponseAdminGetCategoryAttribute>(productSubCategory);
    }
}