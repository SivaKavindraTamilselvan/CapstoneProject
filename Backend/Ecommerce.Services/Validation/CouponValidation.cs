using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class CouponValidation : ICouponValidation
{
    private readonly ICouponRepsository _couponRepsository;
    public CouponValidation(ICouponRepsository couponRepsository)
    {
        _couponRepsository = couponRepsository;
    }
    public async Task<Coupons?> ValidateCouponCode(string code)
    {
        var coupon = await _couponRepsository.GetCouponByCode(code);
        if(coupon != null)
        {
            throw new DataAlreadyRegisteredException("Coupon Code Is Already Registered");
        }
        return coupon;
    }
}
