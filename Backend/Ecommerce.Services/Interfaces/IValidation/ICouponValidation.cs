using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface ICouponValidation
{
    public Task<Coupons?> ValidateCouponCode(string code);
    public Task<List<Coupons>> ValidateGetAllActiveCoupon(int userId);
    public Task<List<Coupons>> ValidateGetAllAvailableCoupons(decimal cost, List<int> productIds, int userId);
}