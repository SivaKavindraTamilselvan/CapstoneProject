using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class OrderItemRepsository : AbstractRepository<int, OrderItems> ,IOrderItemRepsository
{
    public OrderItemRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}