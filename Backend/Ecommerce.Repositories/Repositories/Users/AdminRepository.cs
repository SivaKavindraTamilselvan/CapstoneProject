using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class AdminRepsository : AbstractRepository<int, AdminUser> ,IAdminUserRepsository
{
    public AdminRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<AdminUser?> GetAdminUserByUserId(int userId)
    {
        var adminUser = await _ecommerceContext.AdminUser.FirstOrDefaultAsync(v=>v.UserId == userId);
        return adminUser;
    }

}