using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IProductAttributeValidation
{
    public Task<AttributeMaster> ValidateAttribute(int attributeId);
    public Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId);
    public Task ValidateProductSubCategoryAttributeForAdmin(int productSubCategoryId, int AttributeId);
    public Task ValidateAttributeName(string name);
}