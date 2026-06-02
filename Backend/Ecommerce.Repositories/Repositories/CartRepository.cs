using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class CartRepsository : AbstractRepository<int, Cart> ,ICartRepsository
{
    public CartRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}