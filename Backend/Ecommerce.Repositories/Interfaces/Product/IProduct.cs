using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductRepsository : IRepository<int, Product>
{
    public Task<(List<Product> items, int totalCount)> GetAdminProduct(RequestAdminProductFilter request);
    public Task<(List<Product> items, int totalCount)> GetVendorProduct(RequestVendorProductFilter request, int vendorid);
    public Task<(List<Product> items, int totalCount)> GetUserProducts(RequestUserProductFilter request);
    public Task<Product?> CheckTheProduct(int ProductVariantId, int Qunatity);
    public Task<Product?> GetProductWithFullDetails(int productId);
}