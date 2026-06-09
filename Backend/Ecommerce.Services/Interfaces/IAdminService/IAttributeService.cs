using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductAttributeService
{

    public Task<ResponseAdminGetCategoryAttribute> ActivateProductSubCategoryAttribute(int subcategoryAttribute);
    public Task<ResponseAdminGetAttribute> ActivateProductAttribute(int attributeId);
    public Task<ResponseAdminGetCategoryAttribute> DectivateProductSubCategoryAttribute(int subcategoryAttribute);
    public Task<ResponseAdminGetAttribute> DeactivateProductAttribute(int attributeId);
    public Task<List<ResponseAdminGetAttribute>> GetAllAttributeAdmin(bool? status, int pageNumber, int pageSize);
    public Task<List<ResponseAdminGetCategoryAttribute>> GetAdminCategoryAttribute(bool? status, int? subcategoryid, int pageNumber, int pageSize);
    public Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO, int adminuserid);
    public Task<ResponseAddProductSubCategoryAttributeDTO> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO, int adminuserid);

}