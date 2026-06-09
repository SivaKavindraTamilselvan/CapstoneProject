using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductSubCategoryAttributeRepsository : AbstractRepository<int, ProductSubCategoryAttribute>, IProductSubCategoryAttributeRepsository
{
    public ProductSubCategoryAttributeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<ProductSubCategoryAttribute?> ValidateProductSubCategoryAttribute(int productSubCategoryAttributeId, int productSubCategoryId)
    {
        return await _ecommerceContext.ProductSubCategoryAttribute.FirstOrDefaultAsync(p => p.ProductSubCategoryId == productSubCategoryId && p.ProductSubCategoryAttributeId == productSubCategoryAttributeId);
    }

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

    public async Task<List<ProductSubCategoryAttribute>> GetAdminCategoryAttribute(bool? status, int? subcategoryid, int pageNumber, int pageSize)
    {
        var query = _ecommerceContext.ProductSubCategoryAttribute.Include(p => p.ProductSubCategory).Include(p => p.AttributeMaster).Include(u => u.AddedByAdminUser).ThenInclude(u => u!.User).AsQueryable();
        if (status.HasValue)
        {
            query = query.Where(p => p.IsActive == status);
        }
        if (subcategoryid.HasValue)
        {
            query = query.Where(p => p.ProductSubCategoryId == subcategoryid);
        }
        return await query.OrderBy(p => p!.ProductSubCategory!.ProductSubCategoryName).ThenBy(p => p!.AttributeMaster!.AttributeName).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}