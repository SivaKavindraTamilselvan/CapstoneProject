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
        var vendorUser = await _ecommerceContext.VendorUser.Include(v=>v.Vendor).FirstOrDefaultAsync(v=>v.UserId == userId);
        return vendorUser;
    }
    public async Task<VendorUser?> GetOwnerVendorUserByVendorId(int vendorId)
    {
        return await _ecommerceContext.VendorUser.Where(v=>v.VendorId == vendorId && v.VendorRoleId == 1).FirstOrDefaultAsync();
    }
}