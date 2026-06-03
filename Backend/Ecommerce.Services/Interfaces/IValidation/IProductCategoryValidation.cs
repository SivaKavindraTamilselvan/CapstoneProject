using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IProductCategoryValidation
{
    public Task<ProductSubCategory> ValidateSubCategory(int subCategoryId);
    public Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId);

}