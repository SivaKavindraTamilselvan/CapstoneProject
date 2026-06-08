using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseGetAllProductCategory> ActivateProductCategory(int productCategoryId)
    {
        var productCategory = await _productCategoryRepsository.Get(productCategoryId);
        if(productCategory==null)
        {
            throw new DataNotFoundException("Product Category is not found");
        }
        if(productCategory.IsActive)
        {
            throw new DataAlreadyRegisteredException("Product category is already active");
        }
        productCategory.IsActive = true;
        await _productCategoryRepsository.Update(productCategoryId,productCategory);
        return _mapper.Map<ResponseGetAllProductCategory>(productCategory);
    }
    public async Task<ResponseGetAllProductSubCategoryName> ActivateProductSubCategory(int productSubCategoryId)
    {
        var productSubCategory = await _productSubCategoryRepsository.Get(productSubCategoryId);
        if(productSubCategory==null)
        {
            throw new DataNotFoundException("Product Sub Category is not found");
        }
        if(productSubCategory.IsActive)
        {
            throw new DataAlreadyRegisteredException("Product Sub category is already active");
        }
        productSubCategory.IsActive = false;
        await _productSubCategoryRepsository.Update(productSubCategoryId,productSubCategory);
        return _mapper.Map<ResponseGetAllProductSubCategoryName>(productSubCategory);
    }
}