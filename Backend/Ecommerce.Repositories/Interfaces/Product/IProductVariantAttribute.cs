using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductVariantAttributeRepsository : IRepository<int,ProductVariantAttribute>
{
    public Task<ProductVariantAttribute?> CheckAttributeAlreadyAdded(int productvariantid,int subcategoryid);
}