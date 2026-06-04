using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IProductCategoryValidation
{
    public Task<ProductSubCategory> ValidateSubCategory(int subCategoryId);
    public Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId);
    public Task<ProductCategory?> ValidateProductCategoryName(string ProductCategoryName);
    public Task<ProductSubCategory?> ValidateProductSubCategoryName(string ProductSubCategoryName);
    public Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttributeForAdmin(int productSubCategoryId, int AttributeId);
    public Task<AttributeMaster> ValidateAttributeName(string name);
}