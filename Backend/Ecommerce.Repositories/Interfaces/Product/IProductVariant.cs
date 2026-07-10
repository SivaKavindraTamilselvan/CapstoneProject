using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductVariantRepsository : IRepository<int, ProductVariant>
{
    public Task<(List<ProductVariant> Items, int TotalCount)> GetAllVariantsForVendor(RequestVendorProductVariantFilter request,int vendorId);
    public Task<(List<ProductVariant> Items, int TotalCount)> GetAllVariantsForAdmin(RequestAdminProductVariantFilter request);
    public Task<ProductVariant?> GetProductByProductVariant(int productVariantId);
}