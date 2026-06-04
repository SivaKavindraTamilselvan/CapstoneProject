using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class OrderRepsository : AbstractRepository<int, Order> ,IOrderRepsository
{
    public OrderRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}