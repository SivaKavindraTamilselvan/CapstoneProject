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
    public async Task<List<ProductSubCategory>> GetAllProductSubCategory(int productId)
    {
        return await _ecommerceContext.ProductSubCategory.Where(p=>p.ProductCategoryId == productId && p.IsActive == true).ToListAsync();
    }
    public async Task<(List<ProductSubCategory>,int totalCount)> GetAllSubProductCategoryForAdmin(bool? status,int? subcategoryId,int pageNumber,int pageSize)
    {
        var query = _ecommerceContext.ProductSubCategory.Include(a=>a.AddedByAdminUser).ThenInclude(u=>u!.User).Include(p=>p.ProductCategory).AsQueryable();
        if(status.HasValue)
        {
            query = query.Where(a=>a.IsActive == status);
        }
        if(subcategoryId.HasValue)
        {
            query = query.Where(q=>q.ProductSubCategoryId == subcategoryId);
        }
        var totalCount = await query.CountAsync();
        var data =  await query.OrderBy(a=>a.ProductSubCategoryName).Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
        return(data,totalCount);
    }
}