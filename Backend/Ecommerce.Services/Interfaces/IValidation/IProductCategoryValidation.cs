using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IProductCategoryValidation
{
    public Task<ProductCategory> ValidateCategory(int categoryId);
    public Task<ProductSubCategory> ValidateSubCategory(int subCategoryId);
    public Task<ProductCategory?> ValidateProductCategoryName(string ProductCategoryName);
    public Task<ProductSubCategory?> ValidateProductSubCategoryName(string ProductSubCategoryName);
}