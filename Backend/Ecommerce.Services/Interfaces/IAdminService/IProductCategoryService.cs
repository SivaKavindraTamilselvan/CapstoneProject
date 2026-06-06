using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductCategoryService
{
    public Task<ResponseAddProductCategoryDTO> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO);
    public Task<ResponseAddProductSubCategoryDTO> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO);
}