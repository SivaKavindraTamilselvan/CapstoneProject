using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductService
{
    public Task<PagedResponse<ResponseGetAllProductVariant>> GetAllProductVariant(RequestAdminProductVariantFilter filter);
    public Task<ResponseReviewOfProductDTO> DeleteProduct(RequestDeleteProductDTO requestDeleteProductDTO,int adminUserId);
    public Task<ResponseReviewOfProductVariantDTO> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO, int adminUserId);
    public Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO, int adminUserId);
    public Task<PagedResponse<ResponseAdminGetAllProductDTO>> GetAllProductsForAdmin(RequestAdminProductFilter request);
    Task<ResponseAdminGetAllProductDTO> GetProductWithFullDetails(int productId);

}