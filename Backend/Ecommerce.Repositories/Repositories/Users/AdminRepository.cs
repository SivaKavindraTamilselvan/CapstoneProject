using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class AdminRepsository : AbstractRepository<int, AdminUser>, IAdminUserRepsository
{
    public AdminRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<AdminUser?> GetAdminUserByUserId(int userId)
    {
        var adminUser = await _ecommerceContext.AdminUser.Include(u=>u.AdminRole).Include(u=>u.User).FirstOrDefaultAsync(v => v.UserId == userId);
        return adminUser;
    }
    public async Task<AdminUser?> GetAdminUserByAdminUserId(int adminUserId)
    {
        var adminUser = await _ecommerceContext.AdminUser.Include(u=>u.AdminRole).Include(u=>u.User).FirstOrDefaultAsync(v => v.AdminUserId==adminUserId);
        return adminUser;
    }
    public async Task<List<int>> GetProductAdminUserIds()
    {
        return await _ecommerceContext.AdminUser.Include(u=>u.User).Where(u=>(u.AdminRoleId == 2 || u.AdminRoleId == 1) && u.IsActive).Select(u=>u.User!.UserId).ToListAsync();
    }
    public async Task<List<AdminUser>> GetAllAdminUser(int? role, bool? status, int pageNumber, int pageSize)
    {
        var query = _ecommerceContext.AdminUser.Include(u => u.User).Include(u => u.AdminRole).AsQueryable();
        if (role.HasValue)
        {
            query = query.Where(u => u.AdminRoleId == role.Value);
        }
        if (status.HasValue)
        {
            query = query.Where(u => u.IsActive == status.Value);
        }
        return await query.OrderBy(u => u.AdminRoleId).ThenByDescending(u => u.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

}