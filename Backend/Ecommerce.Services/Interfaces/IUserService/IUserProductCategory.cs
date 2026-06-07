using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;
public interface IUserProductCategoryService
{
    public Task<List<ResponseGetAllProductCategory>> GetAllProductCategory();
    public Task<List<ResponseGetAllAttributeName>> GetAllAttributeNames();
    public Task<List<ResponseGetAllProductSubCategoryAttributeName>> GetAllProductSubCategoryAttributeNames(RequestGetAllProductSubCategoryAttributeName requestGetAllProductSubCategoryAttributeName);
    public Task<List<ResponseGetAllProductSubCategoryName>> GetAllProductSubCategoryNames(RequestGetAllProductSubCategoryName requestGetAllProductSubCategoryName);
    public Task<List<ResponseGetAllProductSubCategoryNameVendor>> GetAllProductSubCategoryNamesVendor(RequestGetAllProductSubCategoryName requestGetAllProductSubCategoryName);
}