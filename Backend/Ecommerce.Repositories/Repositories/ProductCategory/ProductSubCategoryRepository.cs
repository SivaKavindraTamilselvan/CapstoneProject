using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductSubCategoryRepsository : AbstractRepository<int, ProductSubCategory> ,IProductSubCategoryRepsository
{
    public ProductSubCategoryRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<ProductSubCategory?> CheckUniqueProductSubCategory(string productSubCategoryName)
    {
        var product = await _ecommerceContext.ProductSubCategory.FirstOrDefaultAsync(p=>p.ProductSubCategoryName == productSubCategoryName);
        return product;
    }

    // only for the get usage for user
    public async Task<List<ProductSubCategory>> GetAllProductSubCategory(int productId)
    {
        return await _ecommerceContext.ProductSubCategory.Include(p=>p.ProductCategory).Where(p=>p.ProductCategoryId == productId && p.IsActive == true && p!.ProductCategory!.IsActive == true).ToListAsync();
    }

    // get usage for admin
    public async Task<(List<ProductSubCategory>,int totalCount)> GetAllSubProductCategoryForAdmin(bool? status,int? categoryId,int pageNumber,int pageSize)
    {
        var query = _ecommerceContext.ProductSubCategory.Include(a=>a.AddedByAdminUser).ThenInclude(u=>u!.User).Include(p=>p.ProductCategory).AsQueryable();
        if(status.HasValue)
        {
            query = query.Where(a=>a.IsActive == status);
        }
        if(categoryId.HasValue)
        {
            query = query.Where(q=>q.ProductCategoryId == categoryId);
        }
        var totalCount = await query.CountAsync();
        var data =  await query.OrderBy(a=>a.ProductSubCategoryName).Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
        return(data,totalCount);
    }
}