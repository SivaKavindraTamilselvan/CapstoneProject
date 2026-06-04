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
        return await _ecommerceContext.Coupons.FirstOrDefaultAsync(c=>c.CouponCode == code);
    }
}