using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ProductCategoryValidation : IProductCategoryValidation
{
    private readonly IProductCategoryRepsository _productCategoryRepsository;
    private readonly IProductSubCategoryRepsository _productSubCategoryRepsository;
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    public ProductCategoryValidation(IProductCategoryRepsository productCategoryRepsository,IProductSubCategoryRepsository productSubCategoryRepsository, IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository)
    {
        _productCategoryRepsository = productCategoryRepsository;
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
    public async Task<ProductCategory?> ValidateProductCategoryName(string ProductCategoryName)
    {
        var product = await _productCategoryRepsository.CheckUniqueProductCategory(ProductCategoryName);
        if(product !=null)
        {
            throw new DataAlreadyRegisteredException("Already The Product Category Name Is Registered");
        }
        return product;
    }
    public async Task<ProductSubCategory?> ValidateProductSubCategoryName(string ProductSubCategoryName)
    {
        var product = await _productSubCategoryRepsository.CheckUniqueProductSubCategory(ProductSubCategoryName);
        if(product !=null)
        {
            throw new DataAlreadyRegisteredException("Already The Product Sub Category Name Is Registered");
        }
        return product;
    }
    public async Task<ProductSubCategoryAttribute> ValidateProductSubCategoryAttributeForAdmin(int productSubCategoryId,int AttributeId)
    {
        var result = await _productSubCategoryAttributeRepsository.ValidateProductSubCategoryAttribute(AttributeId,productSubCategoryId);
        if(result == null)
        {
            throw new DataNotFoundException("The Sub Category is not mapped to the Attribute");
        }
        return result;
    }
}
