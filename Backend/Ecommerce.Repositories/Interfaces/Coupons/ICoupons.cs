using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface ICouponRepsository : IRepository<int, Coupons>
{
    public Task<Coupons?> GetCouponByCode(string code);
    public Task<List<Coupons>> GetAllAvailableCoupon(decimal totalCartAmount, List<int> productIds, int userId);
    public Task<List<Coupons>> GetAllActiveCoupon(int userId);

}