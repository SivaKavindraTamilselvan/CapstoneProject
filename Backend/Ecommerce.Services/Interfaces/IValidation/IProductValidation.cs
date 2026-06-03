using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IProductValidation
{
    public Task<ProductSubCategory> ValidateSubCategory(int subCategoryId);
    public Task<Product> ValidateProduct(int productId);
    public Task<Product> ValidateProductIfApproved(int productId);
    public Task<ProductVariant> ValidateProductVariant(int productVariantId);
    public Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId);
}