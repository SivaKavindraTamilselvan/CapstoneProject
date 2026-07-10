using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class RefundRepsository : AbstractRepository<int, Refund> ,IRefundRepsository
{
    public RefundRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}