using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CouponProductRepsository : AbstractRepository<int, CouponsProduct>, ICouponProductRepsository
{
    public CouponProductRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}