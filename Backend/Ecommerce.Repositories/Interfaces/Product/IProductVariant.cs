using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductVariantRepsository : IRepository<int, ProductVariant>
{
     public Task<(List<ProductVariant> Items, int TotalCount)> GetAllVariantsForVendor(int vendorUserId, ProductVariantFilterDto filter);
    public Task<(List<ProductVariant> Items, int TotalCount)> GetAllVariantsForAdmin(ProductVariantFilterDto filter);
    public Task<ProductVariant?> GetProductByProductVariant(int productVariantId);
}