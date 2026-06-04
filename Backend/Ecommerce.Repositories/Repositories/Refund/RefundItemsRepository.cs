using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class RefundItemsRepsository : AbstractRepository<int, RefundItems> ,IRefundItemRepsository
{
    public RefundItemsRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}