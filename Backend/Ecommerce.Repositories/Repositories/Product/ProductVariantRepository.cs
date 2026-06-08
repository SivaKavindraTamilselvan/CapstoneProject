using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductVariantRepsository : AbstractRepository<int, ProductVariant> ,IProductVariantRepsository
{
    public ProductVariantRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<ProductVariant?> GetProductByProductVariant(int productVariantId)
    {
        return await _ecommerceContext.ProductVariant.Include(p=>p.Product).ThenInclude(v=>v.Vendor).FirstOrDefaultAsync(p=>p.ProductVariantId == productVariantId);
    }
}