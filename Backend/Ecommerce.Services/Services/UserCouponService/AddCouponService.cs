using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class UserCouponService : IUserCouponService
{
    private readonly ICouponRepsository _couponRepsository;
    private readonly ICouponUsageRepsository _couponUsageRepsository;
    public UserCouponService(ICouponRepsository couponRepsository,ICouponUsageRepsository couponUsageRepsository)
    {
        _couponRepsository = couponRepsository;
        _couponUsageRepsository = couponUsageRepsository;
    }
}