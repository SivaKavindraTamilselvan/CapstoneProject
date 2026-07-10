using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ProductCategoryValidation : IProductCategoryValidation
{
    private readonly IProductCategoryRepsository _productCategoryRepsository;
    private readonly IProductSubCategoryRepsository _productSubCategoryRepsository;
    public ProductCategoryValidation(IProductCategoryRepsository productCategoryRepsository, IProductSubCategoryRepsository productSubCategoryRepsository)
    {
        _productCategoryRepsository = productCategoryRepsository;
        _productSubCategoryRepsository = productSubCategoryRepsository;
    }
    public async Task<ProductSubCategory> ValidateSubCategory(int subCategoryId)
    {
        var subCategory = await _productSubCategoryRepsository.Get(subCategoryId);
        if (subCategory == null)
        {
            throw new DataNotFoundException("Product Sub Category Is Not Found");
        }
        if (!subCategory.IsActive)
        {
            throw new DataAlreadyRegisteredException("Sub category is deactivated");
        }
        return subCategory;
    }
    public async Task<ProductCategory> ValidateCategory(int categoryId)
    {
        var category = await _productCategoryRepsository.Get(categoryId);
        if (category == null)
        {
            throw new DataNotFoundException("Product Category Is Not Found");
        }
        if (!category.IsActive)
        {
            throw new DataAlreadyRegisteredException("Category is deactivated");
        }
        return category;
    }
    public async Task<ProductCategory?> ValidateProductCategoryName(string ProductCategoryName)
    {

        var product = await _productCategoryRepsository.CheckUniqueProductCategory(ProductCategoryName);
        if (product != null && !product.IsActive)
        {
            throw new DataAlreadyRegisteredException("Category is deactivated");
        }
        if (product != null)
        {
            throw new DataAlreadyRegisteredException("Already The Product Category Name Is Registered");
        }
        return product;
    }
    public async Task<ProductSubCategory?> ValidateProductSubCategoryName(string ProductSubCategoryName)
    {
        var product = await _productSubCategoryRepsository.CheckUniqueProductSubCategory(ProductSubCategoryName);
        if (product != null && !product.IsActive)
        {
            throw new DataAlreadyRegisteredException("Sub Category is deactivated");
        }
        if (product != null)
        {
            throw new DataAlreadyRegisteredException("Already The Product Sub Category Name Is Registered");
        }
        return product;
    }
}
