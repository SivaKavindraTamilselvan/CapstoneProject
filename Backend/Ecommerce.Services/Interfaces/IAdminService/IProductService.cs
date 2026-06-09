using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductService
{
    public Task<ResponseReviewOfProductVariantDTO> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO, int adminUserId);
    public Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO, int adminUserId);
    Task<List<ResponseAdminGetAllProductDTO>> GetAllProducts(int? approval,int? status,int? vendorId,int? subcategory,bool? hasIssues);
    Task<List<ResponseAdminGetStockProductDTO>> GetAllOutOfStockProducts();
    Task<List<ResponseAdminGetStockProductDTO>> GetAllLowStockProducts(int threshold = 5);
    Task<List<ResponseAdminGetAllProductDTO>> GetAllProductsWithPendingVariants();
    Task<ResponseAdminGetAllProductDTO> GetProductWithFullDetails(int productId);

}