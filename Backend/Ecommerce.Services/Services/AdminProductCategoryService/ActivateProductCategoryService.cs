using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseAdminGetAllCategory> ActivateProductCategory(int productCategoryId)
    {
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
        await _productCategoryRepsository.Update(productCategoryId, productCategory);

        _logger.LogInformation("Product Category {CategoryId} - {CategoryName} activated successfully",productCategory.ProductCategoryId,productCategory.ProductCategoryName);
        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();

        _logger.LogInformation("Sending category activation notification to {VendorCount} vendors",vendorOwnerIds.Count);
        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "Category Activated",
                $"Product category '{productCategory.ProductCategoryName}' has been activated and is now available for use.",
                notificationTypeId: 1,
                referenceType: "ProductCategory",
                referenceId: productCategory.ProductCategoryId);
        }
        return _mapper.Map<ResponseAdminGetAllCategory>(productCategory);
    }
    public async Task<ResponseAdminGetAllSubCategory> ActivateProductSubCategory(int productSubCategoryId)
    {
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
        productSubCategory.IsActive = false;
        _logger.LogInformation("Product SubCategory {SubCategoryId} - {SubCategoryName} activated successfully",productSubCategory.ProductSubCategoryId,productSubCategory.ProductSubCategoryName);

        var vendorOwnerIds = await _vendorRepsository.GetAllVendorOwnerUserIds();
        _logger.LogInformation("Sending subcategory activation notification to {VendorCount} vendors",vendorOwnerIds.Count);

        foreach (var vendorUserId in vendorOwnerIds)
        {
            await _notificationService.SendToUser(
                vendorUserId,
                "SubCategory Activated",
                $"Product subcategory '{productSubCategory.ProductSubCategoryName}' has been activated and is now available for use.",
                notificationTypeId: 1,
                referenceType: "ProductSubCategory",
                referenceId: productSubCategory.ProductSubCategoryId);
        }
        await _productSubCategoryRepsository.Update(productSubCategoryId, productSubCategory);
        return _mapper.Map<ResponseAdminGetAllSubCategory>(productSubCategory);
    }
}