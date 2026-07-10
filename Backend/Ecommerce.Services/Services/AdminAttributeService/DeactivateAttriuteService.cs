using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<ResponseAdminGetAttribute> DeactivateProductAttribute(int attributeId,int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Attribute deactivation initiated for AttributeId {AttributeId}", attributeId);

        var attribute = await _productAttributeValidation.ValidateAttribute(attributeId);
        attribute.IsActive = false;
        await _attributeRepsository.Update(attribute.AttributeMasterId, attribute);

        _logger.LogInformation("AttributeId {AttributeId} ({AttributeName}) deactivated successfully", attribute.AttributeMasterId, attribute.AttributeName);

        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

        _logger.LogInformation("Sending deactivation notification to {VendorCount} vendors", vendorOwnerIds.Count);

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "Category Attribute Updated",
                $"A new attribute {attribute.AttributeName} has been deactivated. Please review your products.",
                notificationTypeId: 1,
                referenceType: "AttributeMaster",
                referenceId: attribute.AttributeMasterId);
        }
        _logger.LogInformation("Attribute deactivation process completed for AttributeId {AttributeId}", attribute.AttributeMasterId);
        return _mapper.Map<ResponseAdminGetAttribute>(attribute);
    }
    public async Task<ResponseAdminGetCategoryAttribute> DectivateProductSubCategoryAttribute(int subcategoryAttribute,int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("ProductSubCategoryAttribute deactivation initiated for Id {ProductSubCategoryAttributeId}", subcategoryAttribute);
        var productSubCategory = await _productSubCategoryAttributeRepsository.Get(subcategoryAttribute);
        if (productSubCategory == null)
        {
            _logger.LogWarning("ProductSubCategoryAttribute not found for Id {ProductSubCategoryAttributeId}", subcategoryAttribute);
            throw new DataNotFoundException("Product Sub Category Attribute is not found");
        }
        if (!productSubCategory.IsActive)
        {
            _logger.LogWarning("ProductSubCategoryAttributeId {ProductSubCategoryAttributeId} is already inactive", productSubCategory.ProductSubCategoryAttributeId);

            throw new DataAlreadyRegisteredException("Product Sub category is already deactive");
        }
        productSubCategory.IsActive = false;
        await _productSubCategoryAttributeRepsository.Update(subcategoryAttribute, productSubCategory);
        _logger.LogInformation("ProductSubCategoryAttributeId {ProductSubCategoryAttributeId} deactivated successfully", productSubCategory.ProductSubCategoryAttributeId);
        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();
        _logger.LogInformation("Sending subcategory attribute deactivation notification to {VendorCount} vendors", vendorOwnerIds.Count);

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "Category Attribute Updated",
                $"A attribute has been mapped to a product subcategory {productSubCategory.ProductSubCategoryId} is deactivated. Please review your products.",
                notificationTypeId: 1,
                referenceType: "AttributeMaster",
                referenceId: productSubCategory.ProductSubCategoryAttributeId);
        }

        _logger.LogInformation("ProductSubCategoryAttribute deactivation process completed for Id {ProductSubCategoryAttributeId}", productSubCategory.ProductSubCategoryAttributeId);
        return _mapper.Map<ResponseAdminGetCategoryAttribute>(productSubCategory);
    }
}