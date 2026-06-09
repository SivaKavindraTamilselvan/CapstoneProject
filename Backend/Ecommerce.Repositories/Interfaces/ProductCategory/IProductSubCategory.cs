using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductSubCategoryRepsository : IRepository<int, ProductSubCategory>
{
    public Task<(List<ProductSubCategory>, int totalCount)> GetAllSubProductCategoryForAdmin(bool? status, int? categoryId, int pageNumber, int pageSize);
    public Task<List<ProductSubCategory>> GetAllProductSubCategory(int productId);
    public Task<ProductSubCategory?> CheckUniqueProductSubCategory(string productSubCategoryName);
}