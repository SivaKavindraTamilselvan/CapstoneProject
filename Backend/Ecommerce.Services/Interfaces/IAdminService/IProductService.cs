using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductService
{
    public Task<ResponseReviewOfProductVariantDTO> DeleteProductVaraint(RequestDeleteVariantDTO requestDeleteProductDTO, int adminUserId);
    public Task<List<ApprovalHistoryDto>> GetProductHistory(int entityId);
    public Task<List<ApprovalHistoryDto>> GetProductVariantHistory(int entityId);
    public Task<ResponseVendorGetProductVariantOnly> GetProductVariantWithFullDetails(int productVariantId, int adminUserId);
    public Task<PagedResponse<ResponseAdminProductVariantOnlyDTO>> GetAllProductVariant(RequestAdminProductVariantFilter filter, int adminuserid);
    public Task<ResponseReviewOfProductDTO> DeleteProduct(RequestDeleteProductDTO requestDeleteProductDTO, int adminUserId);
    public Task<ResponseReviewOfProductVariantDTO> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO, int adminUserId);
    public Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO, int adminUserId);
    public Task<PagedResponse<ResponseAdminGetAllProductDTO>> GetAllProductsForAdmin(RequestAdminProductFilter request, int adminuserid);
    Task<ResponseAdminGetAllProductDTO> GetProductWithFullDetails(int productId, int adminUserId);

}