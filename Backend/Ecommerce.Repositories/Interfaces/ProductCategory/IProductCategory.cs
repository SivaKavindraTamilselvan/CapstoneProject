using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductCategoryRepsository : IRepository<int,ProductCategory>
{
    public Task<(List<ProductCategory>,int totalCount)> GetAllProductCategoryForAdmin(bool? status,int pageNumber,int pageSize);
    public Task<List<ProductCategory>> GetAllProductCategoryForUser();
    public Task<ProductCategory?> CheckUniqueProductCategory(string productCategoryName);
}