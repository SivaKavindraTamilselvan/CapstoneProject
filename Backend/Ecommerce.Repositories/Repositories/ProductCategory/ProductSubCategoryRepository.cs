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
}