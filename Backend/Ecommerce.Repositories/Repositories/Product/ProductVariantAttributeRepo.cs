using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ProductVariantAttributeRepsository : AbstractRepository<int, ProductVariantAttribute> ,IProductVariantAttributeRepsository
{
    public ProductVariantAttributeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}