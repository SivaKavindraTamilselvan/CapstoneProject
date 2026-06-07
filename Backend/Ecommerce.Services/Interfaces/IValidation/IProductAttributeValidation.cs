using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IProductAttributeValidation
{
    public Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId);
    public Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttributeForAdmin(int productSubCategoryId, int AttributeId);
    public Task ValidateAttributeName(string name);
}