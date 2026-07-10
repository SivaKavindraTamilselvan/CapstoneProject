using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class CancelRefundRepsository : AbstractRepository<int, CancelRefund> ,ICancelRefundRepsository
{
    public CancelRefundRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}