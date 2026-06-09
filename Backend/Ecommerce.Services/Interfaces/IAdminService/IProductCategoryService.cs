using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductCategoryService
{
    public Task<ResponseAdminGetAllCategory> ActivateProductCategory(int productCategoryId);
    public Task<ResponseAdminGetAllSubCategory> ActivateProductSubCategory(int productSubCategoryId);
    public Task<ResponseAdminGetAllSubCategory> DeactivateProductSubCategory(int productSubCategoryId);
    public Task<ResponseAdminGetAllCategory> DeactivateProductCategory(int productCategoryId);
    public Task<List<ResponseAdminGetAllSubCategory>> GetAllProductSubCategoryForAdminGetAllSubProductCategoryForAdmin(bool? status, int? categoryId, int pageNumber, int pageSize);
    public Task<List<ResponseAdminGetAllCategory>> GetAllProductCategoryForAdmin(bool? status, int pageNumber, int pageSize);
    public Task<ResponseAddProductCategoryDTO> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO, int adminuserid);
    public Task<ResponseAddProductSubCategoryDTO> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO, int adminuserid);
}