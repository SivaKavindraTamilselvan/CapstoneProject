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
        var result = await _ecommerceContext.ProductSubCategoryAttribute.FirstOrDefaultAsync(p => p.ProductSubCategoryAttributeId == productSubCategoryAttributeId && p.ProductSubCategoryId == productSubCategoryId);
        return result;
    }

}