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
}