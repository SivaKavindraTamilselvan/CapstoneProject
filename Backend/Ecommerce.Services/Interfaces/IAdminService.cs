using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminService
{
    public Task<ResponseReviewOfVendorDTO> ReviewVendor(RequestReviewOfVendorDTO requestReviewOfVendorDTO, int userId);
    public Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO, int adminUserId);
    public Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId);
    public Task<ResponseAddProductCategoryDTO> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO);
    public Task<ResponseAddProductSubCategoryDTO> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO);
    public Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO);
    public Task<ResponseAddProductSubCategoryAttributeDTO> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO);

}