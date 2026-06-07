using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductRepsository : IRepository<int, Product>
{
    public Task<List<Product>> GetAllAvailableProducts();
    public Task<List<Product>> GetAllPendingAdminApprovalProducts();
    public Task<List<Product>> GetAllPendingVendorApprovalProducts();
    public Task<List<Product>> GetAllAdminApprovedProducts();
    public Task<List<Product>> GetAllAdminRejectedProducts();
    public Task<List<Product>> GetAllVendorRejectedProducts();
    public Task<List<Product>> GetAllDeletedByAdminProducts();
    public Task<List<Product>> GetAllDraftProducts();
    public Task<List<Product>> GetAllActiveProducts();
    public Task<List<Product>> GetAllTemporarilyUnavailableProducts();
    public Task<List<Product>> GetAllArchivedProducts();
    public Task<List<Product>> GetAllProductsByVendorId(int vendorId);
    public Task<List<Product>> GetAllAvailableProductsByVendorId(int vendorId);
    public Task<List<Product>> GetAllAvailableProductsBySubCategoryId(int subCategoryId);
    public Task<List<Product>> GetAllAvailableProductsByCategoryId(int categoryId);
    public Task<List<Product>> GetAllOutOfStockProducts();
    public Task<List<Product>> GetAllLowStockProducts(int threshold = 5);
    public Task<List<Product>> SearchProductsByName(string searchTerm);
    public Task<Product?> GetProductWithFullDetails(int productId);
    public Task<List<Product>> GetAllProductsWithPendingVariants();
}