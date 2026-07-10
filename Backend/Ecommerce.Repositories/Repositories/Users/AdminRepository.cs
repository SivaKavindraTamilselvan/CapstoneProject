using Ecommerce.Data;
using Ecommerce.DTOs;
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
        var adminUser = await _ecommerceContext.AdminUser.Include(u => u.AdminRole).Include(u => u.User).FirstOrDefaultAsync(v => v.UserId == userId);
        return adminUser;
    }
    public async Task<AdminUser?> GetAdminUserByAdminUserId(int adminUserId)
    {
        var adminUser = await _ecommerceContext.AdminUser.Include(u => u.AdminRole).Include(u => u.User).FirstOrDefaultAsync(v => v.AdminUserId == adminUserId);
        return adminUser;
    }
    public async Task<List<int>> GetProductAdminUserIds()
    {
        return await _ecommerceContext.AdminUser.Include(u => u.User).Where(u => (u.AdminRoleId == 2 || u.AdminRoleId == 1) && u.IsActive).Select(u => u.User!.UserId).ToListAsync();
    }
    public async Task<(List<AdminUser> items, int totalCount)> GetAllAdminUser(RequestAdiminUserFilter request)
    {
        var query = _ecommerceContext.AdminUser.Include(u => u.User).Include(u => u.AdminRole).AsQueryable();
        if (request.AdminRoleId.HasValue)
        {
            query = query.Where(u => u.AdminRoleId == request.AdminRoleId.Value);
        }
        if (request.Status.HasValue)
        {
            query = query.Where(u => u.IsActive == request.Status.Value);
        }
        if (!string.IsNullOrEmpty(request.Email))
        {
            query = query.Where(u => u.User!.Email == request.Email);
        }
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            query = query.Where(u => u.User!.PhoneNumber == request.PhoneNumber);
        }
        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(u => u.AdminRoleId).ThenByDescending(u => u.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items, totalCount);
    }

}