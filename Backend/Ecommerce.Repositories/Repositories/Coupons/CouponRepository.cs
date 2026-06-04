using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CouponRepsository : AbstractRepository<int, Coupons>, ICouponRepsository
{
    public CouponRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    public async Task<Coupons?> GetCouponByCode(string code)
    {
        return await _ecommerceContext.Coupons.FirstOrDefaultAsync(c => c.CouponCode == code);
    }
    public async Task<List<Coupons>> GetAllActiveCoupon(int userId)
    {
        var coupon = await _ecommerceContext.Coupons.Include(c => c.CouponsProducts).ThenInclude(cp => cp.Product).Include(cu => cu.CouponUsages).Where(c => c.IsActive == true && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now).ToListAsync();
        coupon = coupon.Where(c => c.CouponUsages.Count(cu => cu.Order!.UserId == userId) < c.MinimumNumberOfUsage).ToList();
        return coupon;
    }
    public async Task<List<Coupons>> GetAllAvailableCoupon(decimal totalCartAmount, List<int> productIds, int userId)
    {
        var coupons = await _ecommerceContext.Coupons.Include(c => c.CouponsProducts).ThenInclude(cp => cp.Product).Include(c => c.CouponUsages).ThenInclude(cu => cu.Order)
        .Where(c => c.IsActive && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now && c.MinimumOrderAmount <= totalCartAmount).ToListAsync();
        coupons = coupons.Where(c => c.CouponUsages.Count(cu => cu.Order!.UserId == userId) < c.MinimumNumberOfUsage).ToList();
        var generalCoupons = coupons.Where(c => !c.CouponsProducts.Any(cp => cp.IsActive)).ToList();
        var productCoupons = coupons.Where(c => c.CouponsProducts.Any(cp => cp.IsActive && productIds.Contains(cp.ProductId))).ToList();
        return generalCoupons.Union(productCoupons).ToList();
    }
}