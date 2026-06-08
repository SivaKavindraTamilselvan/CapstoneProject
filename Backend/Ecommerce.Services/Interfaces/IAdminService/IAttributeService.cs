using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductAttributeService
{
    public Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO,int adminuserid);
    public Task<ResponseAddProductSubCategoryAttributeDTO> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO,int adminuserid);

}