using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class UserCouponService : IUserCouponService
{
    public async Task<List<ResponseGetAllCoupon>> GetAllActiveCoupons(int userId)
    {
        var coupons = await _couponValidation.ValidateGetAllActiveCoupon(userId);
        return _mapper.Map<List<ResponseGetAllCoupon>>(coupons);
    }
    public async Task<List<Coupons>> GetAllAvailableCoupons(int userId)
    {
        var cart = await _cartValidation.ValidateGetCartItemsByUserId(userId);
        var cost = cart.Sum(c => c.Qunatity * c.ProductVariant!.Price);
        var productId = cart.Select(c => c.ProductVariant!.ProductId).Distinct().ToList();
        var coupons = await _couponValidation.ValidateGetAllAvailableCoupons(cost,productId,userId);
        return coupons;
    }
}