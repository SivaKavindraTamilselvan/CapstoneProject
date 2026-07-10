using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductCategoryService
{
    public Task<ResponseAdminGetAllCategory> ActivateProductCategory(int productCategoryId,int adminUserId);
    public Task<ResponseAdminGetAllSubCategory> ActivateProductSubCategory(int productSubCategoryId,int adminUserId);
    public Task<ResponseAdminGetAllSubCategory> DeactivateProductSubCategory(int productSubCategoryId,int adminUserId);
    public Task<ResponseAdminGetAllCategory> DeactivateProductCategory(int productCategoryId,int adminUserId);
    public Task<PagedResponse<ResponseAdminGetAllSubCategory>> GetAllSubProductCategoryForAdmin(RequestProductSubCategoryFilter request,int adminUserId);
    public Task<PagedResponse<ResponseAdminGetAllCategory>> GetAllProductCategoryForAdmin(RequestProductCategoryFilter request,int adminUserId);
    public Task<ResponseAddProductCategoryDTO> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO, int adminuserid);
    public Task<ResponseAddProductSubCategoryDTO> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO, int adminuserid);
}