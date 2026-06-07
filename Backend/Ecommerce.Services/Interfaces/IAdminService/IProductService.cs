using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductService
{
    public Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO, int adminUserId);
    Task<List<ResponseAdminGetAllProductDTO>> GetAllProducts();
    Task<List<ResponseAdminGetPendingProductDTO>> GetAllPendingAdminApprovalProducts();
    Task<List<ResponseAdminGetAllProductDTO>> GetAllAdminApprovedProducts();
    Task<List<ResponseAdminGetAllProductDTO>> GetAllAdminRejectedProducts();
    Task<List<ResponseAdminGetAllProductDTO>> GetAllVendorRejectedProducts();
    Task<List<ResponseAdminGetAllProductDTO>> GetAllDeletedByAdminProducts();
    Task<List<ResponseAdminGetAllProductDTO>> GetAllTemporarilyUnavailableProducts();
    Task<List<ResponseAdminGetAllProductDTO>> GetAllArchivedProducts();
    Task<List<ResponseAdminGetStockProductDTO>> GetAllOutOfStockProducts();
    Task<List<ResponseAdminGetStockProductDTO>> GetAllLowStockProducts(int threshold = 5);
    Task<List<ResponseAdminGetAllProductDTO>> GetAllProductsWithPendingVariants();
    Task<ResponseAdminGetAllProductDTO> GetProductWithFullDetails(int productId);

}