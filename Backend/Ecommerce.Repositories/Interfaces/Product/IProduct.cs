using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductRepsository : IRepository<int, Product>
{
    public Task<List<Product>> GetUserProducts(int? categoryId, int? subcategoryId, string? searchTerm, int pageNumber, int pageSize);
    public Task<List<Product>> GetAdminProduct(int? approval, int? status, int? vendorid, int? subcategory, int pageNumber, int pageSize);
    public Task<List<Product>> GetVendorProduct(int? approval, int? status, int vendorid, int? subcategory, int pageNumber, int pageSize);
    public Task<List<Product>> GetAllAvailableProductsByVendorId(int vendorId);
    public Task<List<Product>> GetAllAvailableProductsBySubCategoryId(int subCategoryId);
    public Task<List<Product>> GetAllAvailableProductsByCategoryId(int categoryId);
    public Task<List<Product>> GetAllOutOfStockProducts();
    public Task<List<Product>> GetAllLowStockProducts(int threshold = 5);
    public Task<List<Product>> SearchProductsByName(string searchTerm);
    public Task<Product?> GetProductWithFullDetails(int productId);
    public Task<List<Product>> GetAllProductsWithPendingVariants();
}