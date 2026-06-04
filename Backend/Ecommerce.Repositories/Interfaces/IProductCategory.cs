using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductCategoryRepsository : IRepository<int,ProductCategory>
{
    public Task<ProductCategory?> CheckUniqueProductCategory(string productCategoryName);
}