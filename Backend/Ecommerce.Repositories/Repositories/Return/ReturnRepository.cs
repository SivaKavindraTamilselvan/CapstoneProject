using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ReturnRepsository : AbstractRepository<int, Return> ,IReturnRepsository
{
    public ReturnRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}