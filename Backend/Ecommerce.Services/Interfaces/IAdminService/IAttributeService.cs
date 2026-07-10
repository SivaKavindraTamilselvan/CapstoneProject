using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductAttributeService
{

    public Task<ResponseAdminGetCategoryAttribute> ActivateProductSubCategoryAttribute(int subcategoryAttribute,int adminUserId);
    public Task<ResponseAdminGetAttribute> ActivateProductAttribute(int attributeId,int adminUserId);
    public Task<ResponseAdminGetCategoryAttribute> DectivateProductSubCategoryAttribute(int subcategoryAttribute,int adminUserId);
    public Task<ResponseAdminGetAttribute> DeactivateProductAttribute(int attributeId,int adminUserId);
    public Task<PagedResponse<ResponseAdminGetAttribute>> GetAllAttributeAdmin(RequestAttributeFilter request,int adminUserId);
    public Task<PagedResponse<ResponseAdminGetCategoryAttribute>> GetAdminCategoryAttribute(RequestSubCategoryAttributeFilter request,int adminUserId);
    public Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO, int adminuserid);
    public Task<ResponseAddProductSubCategoryAttributeDTO> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO, int adminuserid);

}