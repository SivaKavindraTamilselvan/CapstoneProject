using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductAttributeService
{

    public Task<ResponseAdminGetCategoryAttribute> ActivateProductSubCategoryAttribute(int subcategoryAttribute);
    public Task<ResponseAdminGetAttribute> ActivateProductAttribute(int attributeId);
    public Task<ResponseAdminGetCategoryAttribute> DectivateProductSubCategoryAttribute(int subcategoryAttribute);
    public Task<ResponseAdminGetAttribute> DeactivateProductAttribute(int attributeId);
    public Task<PagedResponse<ResponseAdminGetAttribute>> GetAllAttributeAdmin(RequestAttributeFilter request);
    public Task<PagedResponse<ResponseAdminGetCategoryAttribute>> GetAdminCategoryAttribute(RequestSubCategoryAttributeFilter request);
    public Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO, int adminuserid);
    public Task<ResponseAddProductSubCategoryAttributeDTO> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO, int adminuserid);

}