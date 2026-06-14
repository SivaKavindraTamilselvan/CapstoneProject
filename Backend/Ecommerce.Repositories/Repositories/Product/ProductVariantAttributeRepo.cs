using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductVariantAttributeRepsository : AbstractRepository<int, ProductVariantAttribute> ,IProductVariantAttributeRepsository
{
    public ProductVariantAttributeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<ProductVariantAttribute?> CheckAttributeAlreadyAdded(int productvariantid,int subcategoryid)
    {
        return await _ecommerceContext.ProductVariantAttribute.Where(p=>p.ProductSubCategoryAttributeId == subcategoryid && p.ProductVariantId == productvariantid).FirstOrDefaultAsync();
    }

}