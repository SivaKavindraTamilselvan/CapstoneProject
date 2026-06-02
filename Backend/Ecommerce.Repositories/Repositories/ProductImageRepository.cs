using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ProductImageRepsository : AbstractRepository<int, ProductImage> ,IProductImageRepsository
{
    public ProductImageRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}