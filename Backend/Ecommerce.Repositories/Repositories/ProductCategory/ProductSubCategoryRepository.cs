using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ProductSubCategoryRepsository : AbstractRepository<int, ProductSubCategory> ,IProductSubCategoryRepsository
{
    public ProductSubCategoryRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}