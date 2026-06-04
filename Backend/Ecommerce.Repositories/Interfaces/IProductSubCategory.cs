using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductSubCategoryRepsository : IRepository<int, ProductSubCategory>
{
    public Task<ProductSubCategory?> CheckUniqueProductSubCategory(string productSubCategoryName);
}