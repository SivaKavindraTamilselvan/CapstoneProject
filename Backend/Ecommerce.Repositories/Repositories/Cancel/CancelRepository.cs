using System.Security.Cryptography.X509Certificates;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CancelRepsository : AbstractRepository<int, Cancel>, ICancelRepsository
{
    public CancelRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    private IQueryable<Cancel> BaseQuery()
    {
        return _ecommerceContext.Cancel.Include(c => c.CancelReason).Include(c => c.CancelStatus)
        .Include(c => c.OrderItems).ThenInclude(oi => oi!.Order).ThenInclude(o => o!.Address)
        .Include(c => c.OrderItems).ThenInclude(oi => oi!.ProductVariant).ThenInclude(pv => pv!.Product).ThenInclude(p => p!.Vendor)
        .AsNoTracking();
    }
    public async Task<(List<Cancel> data, int totalCount)> GetAllCancelsForAdmin(RequestAdminCancelFilter request)
    {
        Console.WriteLine(request.VendorId);
        var query = BaseQuery();
        if (request.CancelStatusId.HasValue)
        {
            query = query.Where(c => c.CancelStatusId == request.CancelStatusId.Value);
        }

        if (request.CancelReasonId.HasValue)
        {
            query = query.Where(c => c.CancelReasonId == request.CancelReasonId.Value);
        }

        if (request.VendorId.HasValue)
        {
            query = query.Where(c => c.OrderItems!.ProductVariant!.Product!.VendorId == request.VendorId.Value);
        }

        if (request.OrderId.HasValue)
        {
            query = query.Where(c => c.OrderItems!.OrderId == request.OrderId.Value);
        }

        if (request.OrderItemId.HasValue)
        {
            query = query.Where(c => c.OrderItemId == request.OrderItemId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(c => c.CancelledDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(c => c.CancelledDate <= request.ToDate.Value);
        }
        if (request.ProductVariantId.HasValue)
        {
            query = query.Where(c => c.OrderItems!.ProductVariantId == request.ProductVariantId);
        }
        var totalCount = await query.CountAsync();

        var data = await query.OrderByDescending(c => c.CancelledDate).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (data, totalCount);
    }
    public async Task<(List<Cancel> data, int totalCount)> GetAllCancelsForVendor(RequestVendorCancelFilter request, int vendorId)
    {
        var query = BaseQuery().Where(o => o.OrderItems!.ProductVariant!.Product!.VendorId == vendorId);

        if (request.CancelStatusId.HasValue)
        {
            query = query.Where(c => c.CancelStatusId == request.CancelStatusId.Value);
        }
        if (request.CancelReasonId.HasValue)
        {
            query = query.Where(c => c.CancelReasonId == request.CancelReasonId.Value);
        }

        if (request.OrderId.HasValue)
        {
            query = query.Where(c => c.OrderItems!.OrderId == request.OrderId.Value);
        }
        if (request.OrderItemId.HasValue)
        {
            query = query.Where(c => c.OrderItemId == request.OrderItemId.Value);
        }
        if (request.FromDate.HasValue)
        {
            query = query.Where(c => c.CancelledDate >= request.FromDate.Value);
        }
        if (request.ToDate.HasValue)
        {
            query = query.Where(c => c.CancelledDate <= request.ToDate.Value);
        }
        if (request.ProductVariantId.HasValue)
        {
            query = query.Where(c => c.OrderItems!.ProductVariantId == request.ProductVariantId);
        }
        var totalCount = await query.CountAsync();

        var data = await query.OrderByDescending(c => c.CancelledDate).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (data, totalCount);
    }
    public async Task<(List<Cancel> data, int totalCount)> GetAllCancelsForUser(RequestUserCancelFilter request, int userId)
    {
        var query = BaseQuery().Where(o => o.OrderItems!.Order!.UserId == userId);

        if (request.CancelStatusId.HasValue)
        {
            query = query.Where(c => c.CancelStatusId == request.CancelStatusId.Value);
        }
        if (request.FromDate.HasValue)
        {
            query = query.Where(c => c.CancelledDate >= request.FromDate.Value);
        }
        if (request.ToDate.HasValue)
        {
            query = query.Where(c => c.CancelledDate <= request.ToDate.Value);
        }
        var totalCount = await query.CountAsync();

        var data = await query.OrderByDescending(c => c.CancelledDate).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (data, totalCount);
    }

    public async Task<Cancel?> GetCancelById(int cancelId)
    {
        return await BaseQuery().Where(c=>c.CancelId == cancelId).FirstOrDefaultAsync();
    }
}