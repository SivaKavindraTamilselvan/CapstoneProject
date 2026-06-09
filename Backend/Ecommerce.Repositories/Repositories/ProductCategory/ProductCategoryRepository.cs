using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductCategoryRepsository : AbstractRepository<int, ProductCategory> ,IProductCategoryRepsository
{
    public ProductCategoryRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<ProductCategory?> CheckUniqueProductCategory(string productCategoryName)
    {
        var product = await _ecommerceContext.ProductCategory.FirstOrDefaultAsync(p=>p.ProductCategoryName == productCategoryName);
        return product;
    }
    public async Task<(List<ProductCategory>,int totalCount)> GetAllProductCategoryForAdmin(bool? status,int pageNumber,int pageSize)
    {
        var query = _ecommerceContext.ProductCategory.Include(u=>u.AddedByAdminUser).ThenInclude(u=>u!.User).AsQueryable();
        if(status.HasValue)
        {
            query = query.Where(a=>a.IsActive == status);
        }
        var totalCount = await query.CountAsync();
        var data =  await query.OrderBy(a=>a.ProductCategoryName).Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
        return(data,totalCount);
    }
    public async Task<List<ProductCategory>> GetAllProductCategoryForUser()
    {
        return await _ecommerceContext.ProductCategory.Include(p=>p.ProductSubCategories.Where(p=>p.IsActive == true)).Where(a=>a.IsActive == true).OrderBy(a=>a.ProductCategoryName).ToListAsync();
    }
}