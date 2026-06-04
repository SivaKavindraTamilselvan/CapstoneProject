using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface ICouponRepsository : IRepository<int, Coupons>
{
    public Task<Coupons?> GetCouponByCode(string code);
}