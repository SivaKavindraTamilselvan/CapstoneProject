using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ReturnItemsRepsository : AbstractRepository<int, ReturnItems> ,IReturnItemRepsository
{
    public ReturnItemsRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}