using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductAttributeService
{
    public Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO);
    public Task<ResponseAddProductSubCategoryAttributeDTO> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO);

}