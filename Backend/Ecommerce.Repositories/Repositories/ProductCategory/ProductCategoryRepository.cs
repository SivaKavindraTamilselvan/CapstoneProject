using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductCategoryRepsository : AbstractRepository<int, ProductCategory>, IProductCategoryRepsository
{
    public ProductCategoryRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    // used for adding the product category
    public async Task<ProductCategory?> CheckUniqueProductCategory(string productCategoryName)
    {
        var product = await _ecommerceContext.ProductCategory.FirstOrDefaultAsync(p => p.ProductCategoryName == productCategoryName);
        return product;
    }
    // get the product category for admin
    public async Task<(List<ProductCategory>, int totalCount)> GetAllProductCategoryForAdmin(RequestProductCategoryFilter request)
    {
        var query = _ecommerceContext.ProductCategory.Include(u => u.AddedByAdminUser).ThenInclude(u => u!.User).AsQueryable();
        if (request.status.HasValue)
        {
            query = query.Where(a => a.IsActive == request.status);
        }
        if (request.AddedByAdminId.HasValue)
        {
            query = query.Where(a => a.AddedByAdminId == request.AddedByAdminId);
        }
        if (!string.IsNullOrWhiteSpace(request.ProductCategoryName))
        {
            query = query.Where(a => a.ProductCategoryName.ToLower() == request.ProductCategoryName.ToLower());
        }
        if (request.ProductCategoryId.HasValue)
        {
            query = query.Where(a => a.ProductCategoryId == request.ProductCategoryId);
        }
        var totalCount = await query.CountAsync();
        var data = await query.OrderBy(a => a.ProductCategoryName).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (data, totalCount);
    }

    // get product category to display user
    public async Task<List<ProductCategory>> GetAllProductCategoryForUser()
    {
        return await _ecommerceContext.ProductCategory.Include(p => p.ProductSubCategories.Where(p => p.IsActive == true)).Where(a => a.IsActive == true).OrderBy(a => a.ProductCategoryName).ToListAsync();
    }
}