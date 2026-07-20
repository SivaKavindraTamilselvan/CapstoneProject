using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserProductCategoryService : IUserProductCategoryService
{
    public async Task<List<ResponseUserGetAllCategory>> GetAllProductCategory()
    {
        _logger.LogInformation("Getting all active product categories.");

        var productCategory = await _productCategoryRepsository.GetAllProductCategoryForUser();

        if (productCategory.Count == 0)
        {
            _logger.LogWarning("No active product categories found.");
            throw new DataNotFoundException("No active Product Category is found");
        }

        _logger.LogInformation("Retrieved {CategoryCount} active product categories.", productCategory.Count);

        return _mapper.Map<List<ResponseUserGetAllCategory>>(productCategory);
    }

    public async Task<List<ResponseUserGetAllSubCategory>> GetAllProductSubCategor(int productCategoryId)
    {
        _logger.LogInformation("Getting active product subcategories for ProductCategoryId: {ProductCategoryId}", productCategoryId);

        var productSubcategory = await _productSubCategoryRepsository.GetAllSubProductCategoryForUser(productCategoryId);

        if (productSubcategory.Count == 0)
        {
            _logger.LogWarning("No active product subcategories found for ProductCategoryId: {ProductCategoryId}", productCategoryId);
            throw new DataNotFoundException("No active Product Category is found");
        }

        _logger.LogInformation("Retrieved {SubCategoryCount} active product subcategories for ProductCategoryId: {ProductCategoryId}", productSubcategory.Count, productCategoryId);

        return _mapper.Map<List<ResponseUserGetAllSubCategory>>(productSubcategory);
    }

    public async Task<List<ResponseVendorGetAllProductSubCategory>> GetAllProductSubCategoryVendor(int productCategoryId)
    {
        _logger.LogInformation("Getting vendor product subcategories for ProductCategoryId: {ProductCategoryId}", productCategoryId);

        var productSubcategory = await _productSubCategoryRepsository.GetAllSubProductCategoryForUser(productCategoryId);

        if (productSubcategory.Count == 0)
        {
            _logger.LogWarning("No active vendor product subcategories found for ProductCategoryId: {ProductCategoryId}", productCategoryId);
            throw new DataNotFoundException("No active Product Category is found");
        }

        _logger.LogInformation("Retrieved {SubCategoryCount} vendor product subcategories for ProductCategoryId: {ProductCategoryId}", productSubcategory.Count, productCategoryId);

        return _mapper.Map<List<ResponseVendorGetAllProductSubCategory>>(productSubcategory);
    }
}