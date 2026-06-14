using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserProductCategoryService :IUserProductCategoryService
{
    public async Task<List<ResponseUserGetAllCategory>> GetAllProductCategory()
    {
        var productCategory  = await _productCategoryRepsository.GetAllProductCategoryForUser();
        if(productCategory.Count == 0)
        {
            throw new DataNotFoundException("No active Product Category is found");
        }
        return _mapper.Map<List<ResponseUserGetAllCategory>>(productCategory);
    }
    public async Task<List<ResponseUserGetAllSubCategory>> GetAllProductSubCategor(int productCategoryId)
    {
        var productSubcategory = await _productSubCategoryRepsository.GetAllSubProductCategoryForUser(productCategoryId);
        if(productSubcategory.Count == 0)
        {
            throw new DataNotFoundException("No active Product Category is found");
        }
        return _mapper.Map<List<ResponseUserGetAllSubCategory>>(productSubcategory);
    }
    public async Task<List<ResponseVendorGetAllProductSubCategory>> GetAllProductSubCategoryVendor(int productCategoryId)
    {
        var productSubcategory = await _productSubCategoryRepsository.GetAllSubProductCategoryForUser(productCategoryId);
        if(productSubcategory.Count == 0)
        {
            throw new DataNotFoundException("No active Product Category is found");
        }
        return _mapper.Map<List<ResponseVendorGetAllProductSubCategory>>(productSubcategory);
    }
}