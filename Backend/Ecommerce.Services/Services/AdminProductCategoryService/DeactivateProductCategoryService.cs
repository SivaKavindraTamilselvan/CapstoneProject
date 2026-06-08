using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseGetAllProductCategory> DeactivateProductCategory(int productCategoryId)
    {
        var productCategory = await _productCategoryValidation.ValidateCategory(productCategoryId);
        productCategory.IsActive = false;
        await _productCategoryRepsository.Update(productCategoryId,productCategory);
        return _mapper.Map<ResponseGetAllProductCategory>(productCategory);
    }
    public async Task<ResponseGetAllProductSubCategoryName> DeactivateProductSubCategory(int productSubCategoryId)
    {
        var productSubCategory = await _productCategoryValidation.ValidateSubCategory(productSubCategoryId);
        productSubCategory.IsActive = false;
        await _productSubCategoryRepsository.Update(productSubCategoryId,productSubCategory);
        return _mapper.Map<ResponseGetAllProductSubCategoryName>(productSubCategory);
    }
}