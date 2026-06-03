using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductSubCategoryAttributeRepsository : IRepository<int, ProductSubCategoryAttribute>
{
    public Task<ProductSubCategoryAttribute?> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId);

}