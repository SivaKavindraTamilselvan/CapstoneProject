using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ProductCategoryValidation : IProductCategoryValidation
{
    private readonly IProductSubCategoryRepsository _productSubCategoryRepsository;
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    public ProductCategoryValidation(IProductSubCategoryRepsository productSubCategoryRepsository, IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository)
    {
        _productSubCategoryRepsository = productSubCategoryRepsository;
        _productSubCategoryAttributeRepsository = productSubCategoryAttributeRepsository;
    }
    public async Task<ProductSubCategory> ValidateSubCategory(int subCategoryId)
    {
        var subCategory = await _productSubCategoryRepsository.Get(subCategoryId);
        if (subCategory == null)
        {
            throw new DataNotFoundException("Product Sub Category Is Not Found");
        }
        return subCategory;
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

}
