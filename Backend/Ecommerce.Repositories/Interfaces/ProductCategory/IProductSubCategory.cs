using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductSubCategoryRepsository : IRepository<int, ProductSubCategory>
{
    public Task<List<ProductSubCategory>> GetAllSubProductCategoryForUser(int productCategoryId);
    public Task<(List<ProductSubCategory>, int totalCount)> GetAllSubProductCategoryForAdmin(RequestProductSubCategoryFilter request);
    public Task<List<ProductSubCategory>> GetAllProductSubCategory(int productId);
    public Task<ProductSubCategory?> CheckUniqueProductSubCategory(string productSubCategoryName);
}