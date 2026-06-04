using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class CartValidation : ICartValidation
{
    private readonly ICouponRepsository _couponRepsository;
    public CartValidation(ICouponRepsository couponRepsository)
    {
        _couponRepsository = couponRepsository;
    }
    
}
