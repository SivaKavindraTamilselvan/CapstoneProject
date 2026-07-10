using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductSubCategoryAttributeRepsository : AbstractRepository<int, ProductSubCategoryAttribute>, IProductSubCategoryAttributeRepsository
{
    public ProductSubCategoryAttributeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    // check in vendor that the attribute needed to be inserted is mapped for the sub category
    public async Task<ProductSubCategoryAttribute?> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId)
    {
        return await _ecommerceContext.ProductSubCategoryAttribute.FirstOrDefaultAsync(p => p.ProductSubCategoryId == productSubCategoryId && p.ProductSubCategoryAttributeId == productSubCategoryAttributeId);
    }

    // check to avoid duplicate insertion by admin
    public async Task<ProductSubCategoryAttribute?> CheckProductSubCategoryAttribute(int attributeid, int subCategoryId)
    {
        return await _ecommerceContext.ProductSubCategoryAttribute.FirstOrDefaultAsync(p => p.AttributeMasterId == attributeid && p.ProductSubCategoryId == subCategoryId);
    }
    //used for filtering (vendor and the user)
    public async Task<List<ProductSubCategoryAttribute>> GetAllProductSubCategoryAttribute(int subCategoryId)
    {
        return await _ecommerceContext.ProductSubCategoryAttribute.Include(P => P.ProductSubCategory).ThenInclude(p => p!.ProductCategory).Include(p => p.AttributeMaster)
        .Where(p => p.ProductSubCategoryId == subCategoryId && p.IsActive == true && p.ProductSubCategory != null && p.ProductSubCategory.IsActive == true && p.ProductSubCategory.ProductCategory != null &&
         p.ProductSubCategory.ProductCategory.IsActive == true && p.AttributeMaster != null && p.AttributeMaster.IsActive == true).ToListAsync();
    }
    // used for admin 
    public async Task<(List<ProductSubCategoryAttribute> items,int totalCount)> GetAdminCategoryAttribute(RequestSubCategoryAttributeFilter request)
    {

        var query = _ecommerceContext.ProductSubCategoryAttribute.Include(p => p.ProductSubCategory).Include(p => p.AttributeMaster).Include(u => u.AddedByAdminUser).ThenInclude(u => u!.User).AsQueryable();
        if (request.status.HasValue)
        {
            query = query.Where(p => p.IsActive == request.status);
        }
        if (request.AttributeMasterId.HasValue)
        {
            query = query.Where(p => p.AttributeMasterId == request.AttributeMasterId);
        }
        if (request.ProductSubCategoryId.HasValue)
        {
            query = query.Where(p => p.ProductSubCategoryId == request.ProductSubCategoryId);
        }
        if (request.AddedByAdminId.HasValue)
        {
            query = query.Where(p => p.AddedByAdminId == request.AddedByAdminId);
        }
        var totalCount = await query.CountAsync();
        var items =  await query.OrderBy(p => p!.ProductSubCategory!.ProductSubCategoryName).ThenBy(p => p!.AttributeMaster!.AttributeName).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items,totalCount);
    }
}