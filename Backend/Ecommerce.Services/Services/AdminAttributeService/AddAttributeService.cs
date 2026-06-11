using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO, int adminuserid)
    {
        _logger.LogInformation("Attribute creation initiated by UserId {UserId} for AttributeName {AttributeName}", adminuserid, requestAddAttributeDTO.AttributeName);

        var admin = await _adminUserValidation.ValidateAdminUserByUserId(adminuserid);
        await _productAttributeValidation.ValidateAttributeName(requestAddAttributeDTO.AttributeName);

        _logger.LogInformation("Validation completed for AttributeName {AttributeName}", requestAddAttributeDTO.AttributeName);

        AttributeMaster createAttribute = new AttributeMaster();
        createAttribute.AttributeName = requestAddAttributeDTO.AttributeName;
        createAttribute.AddedByAdminId = admin.AdminUserId;
        await _attributeRepsository.Create(createAttribute);

        _logger.LogInformation("Attribute created successfully with AttributeMasterId {AttributeMasterId} by AdminUserId {AdminUserId}", createAttribute.AttributeMasterId, admin.AdminUserId);
        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "Category Attribute Updated",
                $"A new attribute {createAttribute.AttributeName} has been created. Please review your products.",
                notificationTypeId: 1,
                referenceType: "AttributeMaster",
                referenceId: createAttribute.AttributeMasterId);
        }

        return _mapper.Map<ResponseAddAttributeDTO>(createAttribute);
    }
    public async Task<ResponseAddProductSubCategoryAttributeDTO> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO, int adminuserid)
    {
        _logger.LogInformation("ProductSubCategoryAttribute creation initiated by UserId {UserId}. AttributeMasterId {AttributeMasterId}, ProductSubCategoryId {ProductSubCategoryId}", adminuserid, requestAddProductSubCategoryAttributeDTO.AttributeMasterId, requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId);

        var admin = await _adminUserValidation.ValidateAdminUserByUserId(adminuserid);
        await _productAttributeValidation.ValidateProductSubCategoryAttributeForAdmin(requestAddProductSubCategoryAttributeDTO.AttributeMasterId, requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId);
        var mappedattribute = await _productSubCategoryAttributeRepsository.CheckProductSubCategoryAttribute(requestAddProductSubCategoryAttributeDTO.AttributeMasterId, requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId);
        if (mappedattribute != null)
        {
            throw new DataAlreadyRegisteredException("Data Already mapped with this inputs");
        }
        _logger.LogInformation("Validation completed for AttributeMasterId {AttributeMasterId} and ProductSubCategoryId {ProductSubCategoryId}", requestAddProductSubCategoryAttributeDTO.AttributeMasterId, requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId);

        ProductSubCategoryAttribute productSubCategoryAttribute = new ProductSubCategoryAttribute();
        productSubCategoryAttribute.AttributeMasterId = requestAddProductSubCategoryAttributeDTO.AttributeMasterId;
        productSubCategoryAttribute.ProductSubCategoryId = requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId;
        productSubCategoryAttribute.AddedByAdminId = admin.AdminUserId;

        await _productSubCategoryAttributeRepsository.Create(productSubCategoryAttribute);

        _logger.LogInformation("ProductSubCategoryAttribute created successfully with Id {ProductSubCategoryAttributeId} by AdminUserId {AdminUserId}", productSubCategoryAttribute.ProductSubCategoryAttributeId, admin.AdminUserId);

        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "Category Attribute Updated",
                "A new attribute has been mapped to a product subcategory. Please review your products.",
                notificationTypeId: 1,
                referenceType: "AttributeMaster",
                referenceId: productSubCategoryAttribute.ProductSubCategoryAttributeId);
        }

        return _mapper.Map<ResponseAddProductSubCategoryAttributeDTO>(productSubCategoryAttribute);
    }
}