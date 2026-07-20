using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class VendorUserRepsository : AbstractRepository<int, VendorUser>, IVendorUserRepsository
{
    public VendorUserRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<VendorUser?> GetVendorUserByVendorUserId(int userId)
    {
        var vendorUser = await _ecommerceContext.VendorUser.Include(u => u.VendorRole).Include(v => v.User).FirstOrDefaultAsync(v => v.VendorUserId == userId);
        return vendorUser;
    }

    public async Task<VendorUser?> CheckUserIsVendorOwner(int userId)
    {
        var vendorUser = await _ecommerceContext.VendorUser.FirstOrDefaultAsync(v => v.UserId == userId && v.VendorRoleId == 1);
        return vendorUser;
    }

    public async Task<VendorUser?> GetVendorUserByUserId(int userId)
    {
        var vendorUser = await _ecommerceContext.VendorUser.Include(v => v.Vendor).FirstOrDefaultAsync(v => v.UserId == userId);
        return vendorUser;
    }
    public async Task<VendorUser?> GetOwnerVendorUserByVendorId(int vendorId)
    {
        return await _ecommerceContext.VendorUser.Where(v => v.VendorId == vendorId && v.VendorRoleId == 1).FirstOrDefaultAsync();
    }

     public async Task<List<VendorUser>> GetOrderVendorUserByVendorId(int vendorId)
    {
        return await _ecommerceContext.VendorUser.Where(v => v.VendorId == vendorId && (v.VendorRoleId == 1 || v.VendorRoleId == 4)).ToListAsync();
    }

    public async Task<List<int>> GetAllProductVendorUserIds()
    {
        return await _ecommerceContext.VendorUser.Where(v => v.VendorRoleId == (int)VendorRoleEnum.Owner || v.VendorRoleId == (int)VendorRoleEnum.ProductManager).Select(v => v.UserId).ToListAsync();
    }
    public async Task<(List<VendorUser> items, int totalCount)> GetVendorsForAdmin(RequestAdminVendorUserFilter request)
    {
        var query = _ecommerceContext.VendorUser.Include(u => u.User).Include(u => u.VendorRole).AsQueryable();
        if (request.VendorRoleId.HasValue)
        {
            query = query.Where(u => u.VendorRoleId == request.VendorRoleId.Value);
        }
        if (request.VendorId.HasValue)
        {
            query = query.Where(u => u.VendorId == request.VendorId.Value);
        }
        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }
        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(u => u.VendorRoleId).ThenByDescending(u => u.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<(List<VendorUser> items, int totalCount)> GetVendorsForVendor(RequestVendorUserFilter request, int vendorId)
    {
        var query = _ecommerceContext.VendorUser.Include(u => u.User).Include(u => u.VendorRole).AsQueryable();
        query = query.Where(v => v.VendorId == vendorId);
        if (request.VendorRoleId.HasValue)
        {
            query = query.Where(u => u.VendorRoleId == request.VendorRoleId.Value);
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
        var items = await query.OrderBy(u => u.VendorRoleId).ThenByDescending(u => u.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items, totalCount);
    }
}