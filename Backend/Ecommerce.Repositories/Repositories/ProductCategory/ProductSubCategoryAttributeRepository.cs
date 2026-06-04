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
        return await _ecommerceContext.ProductSubCategoryAttribute.FirstOrDefaultAsync(p=>p.ProductSubCategoryId == productSubCategoryId && p.ProductSubCategoryAttributeId == productSubCategoryAttributeId);
    }

    public async Task<ProductSubCategoryAttribute?> CheckProductSubCategoryAttribute(int attributeid,int subCategoryId)
    {
        return await _ecommerceContext.ProductSubCategoryAttribute.FirstOrDefaultAsync(p=>p.AttributeMasterId == attributeid && p.ProductSubCategoryId == subCategoryId);
    }
}