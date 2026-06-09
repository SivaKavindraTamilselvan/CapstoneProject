using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<ResponseAdminGetAttribute> ActivateProductAttribute(int attributeId)
    {
        var attribute = await _attributeRepsository.Get(attributeId);
        if(attribute==null)
        {
            throw new DataNotFoundException("Product Category is not found");
        }
        if(attribute.IsActive)
        {
            throw new DataAlreadyRegisteredException("Product category is already active");
        }
        attribute.IsActive = true;
        await _attributeRepsository.Update(attribute.AttributeMasterId,attribute);
        return _mapper.Map<ResponseAdminGetAttribute>(attribute);
    }
    public async Task<ResponseAdminGetCategoryAttribute> ActivateProductSubCategoryAttribute(int subcategoryAttribute)
    {
        var productSubCategory = await _productSubCategoryAttributeRepsository.Get(subcategoryAttribute);
        if(productSubCategory==null)
        {
            throw new DataNotFoundException("Product Sub Category Attribute is not found");
        }
        if(productSubCategory.IsActive)
        {
            throw new DataAlreadyRegisteredException("Product Sub category is already active");
        }
        productSubCategory.IsActive = true;
        await _productSubCategoryAttributeRepsository.Update(subcategoryAttribute,productSubCategory);
        return _mapper.Map<ResponseAdminGetCategoryAttribute>(productSubCategory);
    }
}