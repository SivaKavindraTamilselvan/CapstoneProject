using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseAdminGetAllCategory> DeactivateProductCategory(int productCategoryId)
    {
        var productCategory = await _productCategoryValidation.ValidateCategory(productCategoryId);
        productCategory.IsActive = false;
        await _productCategoryRepsository.Update(productCategoryId,productCategory);
        return _mapper.Map<ResponseAdminGetAllCategory>(productCategory);
    }
    public async Task<ResponseAdminGetAllSubCategory> DeactivateProductSubCategory(int productSubCategoryId)
    {
        var productSubCategory = await _productCategoryValidation.ValidateSubCategory(productSubCategoryId);
        productSubCategory.IsActive = false;
        await _productSubCategoryRepsository.Update(productSubCategoryId,productSubCategory);
        return _mapper.Map<ResponseAdminGetAllSubCategory>(productSubCategory);
    }
}