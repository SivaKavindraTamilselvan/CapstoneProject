using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ProductVariantRepsository : AbstractRepository<int, ProductVariant> ,IProductVariantRepsository
{
    public ProductVariantRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}