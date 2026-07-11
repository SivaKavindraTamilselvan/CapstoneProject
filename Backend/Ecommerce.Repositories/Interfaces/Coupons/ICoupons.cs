using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface ICouponRepsository : IRepository<int, Coupons>
{
        public List<CouponUsageDto> GetUsageHistory(Coupons coupon);
        public  Task<Coupons?> GetCouponByIdAsync(int couponId);
    public Task<(List<Coupons> Coupons, int TotalCount)> GetAdminCouponsAsync(AdminCouponFilter filter);
    public Task<Coupons?> GetCouponByCode(string code);
    public Task<List<Coupons>> GetAllAvailableCoupon(decimal totalCartAmount, List<int> productIds, int userId);
    public Task<List<Coupons>> GetAllActiveCoupon(int userId);

}