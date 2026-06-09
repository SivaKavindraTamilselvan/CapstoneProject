using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IUserProductCategoryService
{
    public Task<List<ResponseUserGetAllCategory>> GetAllProductCategory();
    public Task<List<ResponseGetAllProductSubCategoryAttribute>> GetAllProductSubCategoryAttributeNames(int productSubCategoryId);
    public Task<List<ResponseUserGetAllSubCategory>> GetAllProductSubCategoryNames(int productCategoryId);
    public Task<List<ResponseVendorGetAllProductSubCategory>> GetAllProductSubCategoryNamesVendor(int productCategoryId);

}