using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ReturnRefundRepsository : AbstractRepository<int, ReturnRefund> ,IReturnRefundRepsository
{
    public ReturnRefundRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}