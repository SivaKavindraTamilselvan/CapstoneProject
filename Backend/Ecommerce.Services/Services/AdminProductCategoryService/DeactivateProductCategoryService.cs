using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseAdminGetAllCategory> DeactivateProductCategory(int productCategoryId)
    {
        _logger.LogInformation("Deactivating Product Category {CategoryId}", productCategoryId);

        var productCategory = await _productCategoryValidation.ValidateCategory(productCategoryId);
        productCategory.IsActive = false;
        await _productCategoryRepsository.Update(productCategoryId, productCategory);
        _logger.LogInformation("Product Category {CategoryId} - {CategoryName} deactivated successfully", productCategory.ProductCategoryId, productCategory.ProductCategoryName);
        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

        _logger.LogInformation("Sending category deactivation notification to {VendorCount} vendors", vendorOwnerIds.Count);
        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "Category Dectivated",
                $"Product category '{productCategory.ProductCategoryName}' has been deactivated and is now not available for use.",
                notificationTypeId: 1,
                referenceType: "ProductCategory",
                referenceId: productCategory.ProductCategoryId);
        }

        return _mapper.Map<ResponseAdminGetAllCategory>(productCategory);
    }
    public async Task<ResponseAdminGetAllSubCategory> DeactivateProductSubCategory(int productSubCategoryId)
    {
        _logger.LogInformation("Deactivating Product SubCategory {SubCategoryId}", productSubCategoryId);
        var productSubCategory = await _productCategoryValidation.ValidateSubCategory(productSubCategoryId);
        productSubCategory.IsActive = false;
        await _productSubCategoryRepsository.Update(productSubCategoryId, productSubCategory);
        _logger.LogInformation("Product SubCategory {SubCategoryId} - {SubCategoryName} deactivated successfully", productSubCategory.ProductSubCategoryId, productSubCategory.ProductSubCategoryName);
        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();
        _logger.LogInformation("Sending subcategory deactivation notification to {VendorCount} vendors", vendorOwnerIds.Count);

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "SubCategory Dectivated",
                $"Product subcategory '{productSubCategory.ProductSubCategoryName}' has been deactivated and is now not available for use.",
                notificationTypeId: 1,
                referenceType: "ProductSubCategory",
                referenceId: productSubCategory.ProductSubCategoryId);
        }
        return _mapper.Map<ResponseAdminGetAllSubCategory>(productSubCategory);
    }
}