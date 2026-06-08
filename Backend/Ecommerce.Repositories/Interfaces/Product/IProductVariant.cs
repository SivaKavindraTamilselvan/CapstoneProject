using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductVariantRepsository : IRepository<int,ProductVariant>
{
    public Task<ProductVariant?> GetProductByProductVariant(int productVariantId);
}