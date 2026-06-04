using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ProductRepsository : AbstractRepository<int, Product> ,IProductRepsository
{
    public ProductRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
}