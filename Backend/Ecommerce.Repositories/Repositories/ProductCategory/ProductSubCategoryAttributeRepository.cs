using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ProductSubCategoryAttributeRepsository : AbstractRepository<int, ProductSubCategoryAttribute> ,IProductSubCategoryAttributeRepsository
{
    public ProductSubCategoryAttributeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}