using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ProductAttributeValidation : IProductAttributeValidation
{
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    private readonly IAttributeRepsository _attributeRepsository;
    public ProductAttributeValidation(IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository,IAttributeRepsository attributeRepsository)
    {
        _productSubCategoryAttributeRepsository = productSubCategoryAttributeRepsository;
        _attributeRepsository = attributeRepsository;
    }
    public async Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId)
    {
        var attribute = await _productSubCategoryAttributeRepsository.ValidateProductSubCategoryAttribute(productSubCategoryAttributeId, productSubCategoryId);
        if (attribute == null)
        {
            throw new DataNotFoundException("Product Sub Category Attribute Not Found For This Product Sub Category");
        }
        return attribute;
    }
    public async Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttributeForAdmin(int productSubCategoryId, int AttributeId)
    {
        var result = await _productSubCategoryAttributeRepsository.ValidateProductSubCategoryAttribute(AttributeId, productSubCategoryId);
        if (result == null)
        {
            throw new DataNotFoundException("The Sub Category is not mapped to the Attribute");
        }
        return result;
    }
    public async Task ValidateAttributeName(string name)
    {
        var attribute = await _attributeRepsository.GetTheAttributeByName(name);
        if (attribute != null)
        {
            throw new DataAlreadyRegisteredException("Already The Attribute Name Is Registered");
        }
        return;
    }
}