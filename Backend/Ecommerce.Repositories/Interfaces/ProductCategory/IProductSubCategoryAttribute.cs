using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductSubCategoryAttributeRepsository : IRepository<int, ProductSubCategoryAttribute>
{
    public Task<List<ProductSubCategoryAttribute>> GetAllProductSubCategoryAttribute(int subCategoryId);
    public Task<ProductSubCategoryAttribute?> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId);
    public Task<ProductSubCategoryAttribute?> CheckProductSubCategoryAttribute(int attributeid, int subCategoryId);
    public Task<List<ProductSubCategoryAttribute>> GetAdminCategoryAttribute(bool? status, int? subcategoryid, int pageNumber, int pageSize);
}