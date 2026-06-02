using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class CartItemsRepsository : AbstractRepository<int, CartItems> ,ICartItemsRepsository
{
    public CartItemsRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}