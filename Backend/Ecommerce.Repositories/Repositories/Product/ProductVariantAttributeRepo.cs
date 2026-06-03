using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductVariantAttributeRepsository : AbstractRepository<int, ProductVariantAttribute> ,IProductVariantAttributeRepsository
{
    public ProductVariantAttributeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
}