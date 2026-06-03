using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ProductCategoryRepsository : AbstractRepository<int, ProductCategory> ,IProductCategoryRepsository
{
    public ProductCategoryRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}