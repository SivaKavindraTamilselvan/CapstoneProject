using Ecommerce.Data;
using Ecommerce.DTOs;
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
    public async Task<(List<ProductSubCategory>,int totalCount)> GetAllSubProductCategoryForAdmin(RequestProductSubCategoryFilter request)
    {
        var query = _ecommerceContext.ProductSubCategory.Include(a=>a.AddedByAdminUser).ThenInclude(u=>u!.User).Include(p=>p.ProductCategory).AsQueryable();
        if(request.status.HasValue)
        {
            query = query.Where(a=>a.IsActive == request.status);
        }
        if(request.ProductCategoryId.HasValue)
        {
            query = query.Where(q=>q.ProductCategoryId == request.ProductCategoryId);
        }
        if(request.ProductSubCategoryId.HasValue)
        {
            query = query.Where(q=>q.ProductSubCategoryId == request.ProductSubCategoryId);
        }
        if(request.MinimumCommissionPercentage.HasValue)
        {
            query = query.Where(q=>q.CommissionPercentage >= request.MinimumCommissionPercentage.Value);
        }
        if(request.MaximumCommissionPercentage.HasValue)
        {
            query = query.Where(q=>q.CommissionPercentage <= request.MaximumCommissionPercentage.Value);
        }
        if(request.AddedByAdminId.HasValue)
        {
            query = query.Where(q=>q.AddedByAdminId == request.AddedByAdminId.Value);
        }
        var totalCount = await query.CountAsync();
        var data =  await query.OrderBy(a=>a.ProductSubCategoryName).Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize).ToListAsync();
        return(data,totalCount);
    }
}