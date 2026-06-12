using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductCategoryService
{
    public Task<ResponseAdminGetAllCategory> ActivateProductCategory(int productCategoryId);
    public Task<ResponseAdminGetAllSubCategory> ActivateProductSubCategory(int productSubCategoryId);
    public Task<ResponseAdminGetAllSubCategory> DeactivateProductSubCategory(int productSubCategoryId);
    public Task<ResponseAdminGetAllCategory> DeactivateProductCategory(int productCategoryId);
    public Task<PagedResponse<ResponseAdminGetAllSubCategory>> GetAllSubProductCategoryForAdmin(RequestProductSubCategoryFilter request);
    public Task<PagedResponse<ResponseAdminGetAllCategory>> GetAllProductCategoryForAdmin(RequestProductCategoryFilter request);
    public Task<ResponseAddProductCategoryDTO> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO, int adminuserid);
    public Task<ResponseAddProductSubCategoryDTO> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO, int adminuserid);
}