using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<ResponseAdminGetAttribute> DeactivateProductAttribute(int attributeId)
    {
        var attribute = await _productAttributeValidation.ValidateAttribute(attributeId);
        attribute.IsActive = false;
        await _attributeRepsository.Update(attribute.AttributeMasterId,attribute);
        return _mapper.Map<ResponseAdminGetAttribute>(attribute);
    }
    public async Task<ResponseAdminGetCategoryAttribute> DectivateProductSubCategoryAttribute(int subcategoryAttribute)
    {
        var productSubCategory = await _productSubCategoryAttributeRepsository.Get(subcategoryAttribute);
        if(productSubCategory==null)
        {
            throw new DataNotFoundException("Product Sub Category Attribute is not found");
        }
        if(!productSubCategory.IsActive)
        {
            throw new DataAlreadyRegisteredException("Product Sub category is already deactive");
        }
        productSubCategory.IsActive = false;
        await _productSubCategoryAttributeRepsository.Update(subcategoryAttribute,productSubCategory);
        return _mapper.Map<ResponseAdminGetCategoryAttribute>(productSubCategory);
    }
}