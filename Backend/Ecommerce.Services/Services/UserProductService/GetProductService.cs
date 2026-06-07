using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;

public partial class UserProductCategoryService :IUserProductCategoryService
{
    public async Task<List<ResponseGetAllProductCategory>> GetAllProductCategory()
    {
        var productCategory  = await _productCategoryRepsository.GetAll();
        return _mapper.Map<List<ResponseGetAllProductCategory>>(productCategory);
    }
    public async Task<List<ResponseGetAllAttributeName>> GetAllAttributeNames()
    {
        var attribute = await _attributeRepsository.GetAll();
        return _mapper.Map<List<ResponseGetAllAttributeName>>(attribute);
    }
    public async Task<List<ResponseGetAllProductSubCategoryAttributeName>> GetAllProductSubCategoryAttributeNames(RequestGetAllProductSubCategoryAttributeName requestGetAllProductSubCategoryAttributeName)
    {
        var productSubcategory = await _productSubCategoryAttributeRepsository.GetAllProductSubCategoryAttribute(requestGetAllProductSubCategoryAttributeName.ProductSubCategoryAttributeId);
        return _mapper.Map<List<ResponseGetAllProductSubCategoryAttributeName>>(productSubcategory);
    }
    public async Task<List<ResponseGetAllProductSubCategoryName>> GetAllProductSubCategoryNames(RequestGetAllProductSubCategoryName requestGetAllProductSubCategoryName)
    {
        var productSubcategory = await _productSubCategoryRepsository.GetAllProductSubCategory(requestGetAllProductSubCategoryName.ProductCategoryId);
        return _mapper.Map<List<ResponseGetAllProductSubCategoryName>>(productSubcategory);
    }
    public async Task<List<ResponseGetAllProductSubCategoryNameVendor>> GetAllProductSubCategoryNamesVendor(RequestGetAllProductSubCategoryName requestGetAllProductSubCategoryName)
    {
        var productSubcategory = await _productSubCategoryRepsository.GetAllProductSubCategory(requestGetAllProductSubCategoryName.ProductCategoryId);
        return _mapper.Map<List<ResponseGetAllProductSubCategoryNameVendor>>(productSubcategory);
    }
}