using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CouponVendorRepsository : AbstractRepository<int, CouponsVendor>, ICouponVendorRepsository
{
    public CouponVendorRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}