using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseAddProductCategoryDTO> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO, int adminuserid)
    {
        _logger.LogInformation("Admin UserId {AdminUserId} is adding Product Category {CategoryName}",adminuserid,requestAddProductCategoryDTO.ProductCategoryName);

        var admin = await _adminUserValidation.ValidateAdminUserByUserId(adminuserid);
        await _productCategoryValidation.ValidateProductCategoryName(requestAddProductCategoryDTO.ProductCategoryName);
        ProductCategory productCategory = new ProductCategory();
        productCategory.ProductCategoryName = requestAddProductCategoryDTO.ProductCategoryName;
        productCategory.AddedByAdminId = admin.AdminUserId;
        await _productCategoryRepsository.Create(productCategory);

        _logger.LogInformation("Product Category {CategoryId} - {CategoryName} created successfully",productCategory.ProductCategoryId,productCategory.ProductCategoryName);
        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

        _logger.LogInformation("Sending category creation notification to {VendorCount} vendors",vendorOwnerIds.Count);

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "New Category Added",
                $"A new product category '{productCategory.ProductCategoryName}' has been added and is available for use.",
                notificationTypeId: 1,
                referenceType: "ProductCategory",
                referenceId: productCategory.ProductCategoryId);
        }
        return _mapper.Map<ResponseAddProductCategoryDTO>(productCategory);
    }
    public async Task<ResponseAddProductSubCategoryDTO> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO, int adminuserid)
    {
        _logger.LogInformation("Admin UserId {AdminUserId} is adding Product SubCategory {SubCategoryName}",adminuserid,requestAddProductSubCategoryDTO.ProductSubCategoryName);

        var admin = await _adminUserValidation.ValidateAdminUserByUserId(adminuserid);
        await _productCategoryValidation.ValidateCategory(requestAddProductSubCategoryDTO.ProductCategoryId);
        await _productCategoryValidation.ValidateProductSubCategoryName(requestAddProductSubCategoryDTO.ProductSubCategoryName);
        ProductSubCategory productSubCategory = new ProductSubCategory();
        productSubCategory.ProductCategoryId = requestAddProductSubCategoryDTO.ProductCategoryId;
        productSubCategory.ProductSubCategoryName = requestAddProductSubCategoryDTO.ProductSubCategoryName;
        productSubCategory.AddedByAdminId = admin.AdminUserId;
        await _productSubCategoryRepsository.Create(productSubCategory);
        _logger.LogInformation("Product SubCategory {SubCategoryId} - {SubCategoryName} created successfully",productSubCategory.ProductSubCategoryId,productSubCategory.ProductSubCategoryName);

        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

        _logger.LogInformation(
            "Sending subcategory creation notification to {VendorCount} vendors",
            vendorOwnerIds.Count);

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "New SubCategory Added",
                $"A new product subcategory '{productSubCategory.ProductSubCategoryName}' has been added and is available for use.",
                notificationTypeId: 1,
                referenceType: "ProductSubCategory",
                referenceId: productSubCategory.ProductSubCategoryId);
        }
        return _mapper.Map<ResponseAddProductSubCategoryDTO>(productSubCategory);
    }
}