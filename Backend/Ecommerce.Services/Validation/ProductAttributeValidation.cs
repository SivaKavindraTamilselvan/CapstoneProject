using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ProductAttributeValidation : IProductAttributeValidation
{
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    private readonly IProductCategoryValidation _productCategoryValidation;
    private readonly IAttributeRepsository _attributeRepsository;
    public ProductAttributeValidation(IProductCategoryValidation productCategoryValidation,IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository,IAttributeRepsository attributeRepsository)
    {
        _productSubCategoryAttributeRepsository = productSubCategoryAttributeRepsository;
        _productCategoryValidation = productCategoryValidation;
        _attributeRepsository = attributeRepsository;
    }
    public async Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId)
    {
        await _productCategoryValidation.ValidateSubCategory(productSubCategoryId);
        var attribute = await _productSubCategoryAttributeRepsository.ValidateProductSubCategoryAttribute(productSubCategoryAttributeId, productSubCategoryId);
        if (attribute == null)
        {
            throw new DataNotFoundException("Product Sub Category Attribute Not Found For This Product Sub Category");
        }
        if(!attribute.IsActive)
        {
            throw new DataNotFoundException("Product Attribute is inactivated");
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
        if(!result.IsActive)
        {
            throw new DataAlreadyRegisteredException("Product Sub Category Attribute is inactivated");
        }
        return result;
    }
    public async Task ValidateAttributeName(string name)
    {
        var attribute = await _attributeRepsository.GetTheAttributeByName(name);
        if(attribute!=null && !attribute.IsActive)
        {
            throw new DataAlreadyRegisteredException("Already The Attribute Name is Registered but inactivated");
        }
        if (attribute != null)
        {
            throw new DataAlreadyRegisteredException("Already The Attribute Name Is Registered");
        }
        return;
    }
}