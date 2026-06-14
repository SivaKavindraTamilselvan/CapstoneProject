using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IUserProductCategoryService
{
    public Task<List<ResponseUserGetAllSubCategory>> GetAllProductSubCategor(int productCategoryId);
    public Task<List<ResponseVendorGetAllProductSubCategory>> GetAllProductSubCategoryVendor(int productCategoryId);
    public Task<List<ResponseUserGetAllCategory>> GetAllProductCategory();
}