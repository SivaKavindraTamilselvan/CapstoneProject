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
        if (coupon != null)
        {
            throw new DataAlreadyRegisteredException("Coupon Code Is Already Registered");
        }
        return coupon;
    }
    public async Task<List<Coupons>> ValidateGetAllActiveCoupon(int userId)
    {
        var coupons = await _couponRepsository.GetAllActiveCoupon(userId);
        if (coupons.Count == 0)
        {
            throw new DataNotFoundException("Active Coupins Are Not Found");
        }
        return coupons;
    }
    public async Task<List<Coupons>> ValidateGetAllAvailableCoupons(decimal cost, List<int> productIds, int userId)
    {
        var coupons = await _couponRepsository.GetAllAvailableCoupon(cost, productIds, userId);
        if(coupons.Count == 0)
        {
            throw new DataNotFoundException("No Coupons Are Available For The Particular User");
        }
        return coupons;
    }
    public async Task<Coupons> ValidateCoupon(int couponId)
    {
        var coupon = await _couponRepsository.Get(couponId);
        if(coupon == null)
        {
            throw new DataNotFoundException("Coupon Not Found");
        }
        return coupon;
    }
}
