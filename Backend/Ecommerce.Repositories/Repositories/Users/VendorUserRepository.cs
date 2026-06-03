using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class VendorUserRepsository : AbstractRepository<int, VendorUser> ,IVendorUserRepsository
{
    public VendorUserRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    public async Task<VendorUser?> GetVendorUserByUserId(int userId)
    {
        var vendorUser = await _ecommerceContext.VendorUser.FirstOrDefaultAsync(v=>v.UserId == userId);
        return vendorUser;
    }
}