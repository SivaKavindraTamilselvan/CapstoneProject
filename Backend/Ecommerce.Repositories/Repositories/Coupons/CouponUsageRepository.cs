using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CouponUsageRepsository : AbstractRepository<int, CouponUsage>, ICouponUsageRepsository
{
    public CouponUsageRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}