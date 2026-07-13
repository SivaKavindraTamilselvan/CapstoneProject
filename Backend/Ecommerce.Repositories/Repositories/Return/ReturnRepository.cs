using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ReturnRepsository : AbstractRepository<int, Return>, IReturnRepsository
{
    public ReturnRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<Return?> GetTheReturnInventoryByReturnId(int returnId)
    {
        return await _ecommerceContext.Return.Include(r => r.OrderItems).ThenInclude(o => o!.Inventory).ThenInclude(i => i!.Address).Where(r => r.ReturnId == returnId).FirstOrDefaultAsync();
    }
    public async Task<Return?> GetTheReturnUserByReturnId(int returnId)
    {
        return await _ecommerceContext.Return.Include(r => r.OrderItems).ThenInclude(o => o!.Order).ThenInclude(o => o!.Users).ThenInclude(a => a!.Addresses).Where(r => r.ReturnId == returnId).FirstOrDefaultAsync();
    }
    public async Task<Return?> GetTheReturnProductByReturnId(int returnId)
    {
        return await _ecommerceContext.Return.Include(r => r.OrderItems).ThenInclude(o => o!.ProductVariant).ThenInclude(p => p!.Product).Where(r => r.ReturnId == returnId).FirstOrDefaultAsync();
    }
    public async Task<Return?> GetTheReturnProductByOrderItemId(int orderItemId)
    {
        return await _ecommerceContext.Return.Where(o => o.OrderItemId == orderItemId).FirstOrDefaultAsync();
    }

    private IQueryable<Return> BaseQuery()
    {
        return _ecommerceContext.Return.Include(r => r.ReturnReason).Include(r => r.ReturnStatus)
        .Include(r => r.OrderItems).ThenInclude(oi => oi!.Order).ThenInclude(o => o!.Address)
        .Include(r => r.OrderItems).ThenInclude(oi => oi!.Inventory).ThenInclude(i => i!.Address)
        .Include(r => r.OrderItems).ThenInclude(oi => oi!.ProductVariant).ThenInclude(pv => pv!.Product).ThenInclude(p => p!.Vendor)
        .AsNoTracking();
    }

    public async Task<Return?> GetReturnSummaryById(int returnId)
    {
        var cancel = await BaseQuery().FirstOrDefaultAsync(r => r.ReturnId == returnId);
        return cancel;
    }
    public async Task<(List<Return> data, int totalCount)> GetAllReturnsForAdmin(RequestAdminReturnFilter request)
    {
        var query = BaseQuery();
        if (request.ReturnStatusId.HasValue)
        {
            query = query.Where(r => r.ReturnStatusId == request.ReturnStatusId.Value);
        }

        if (request.ReturnReasonId.HasValue)
        {
            query = query.Where(r => r.ReturnReasonId == request.ReturnReasonId.Value);
        }

        if (request.VendorId.HasValue)
        {
            query = query.Where(r => r.OrderItems!.ProductVariant!.Product!.VendorId == request.VendorId.Value);
        }

        if (request.OrderItemId.HasValue)
        {
            query = query.Where(r => r.OrderItemId == request.OrderItemId.Value);
        }

        if (request.OrderId.HasValue)
        {
            query = query.Where(r => r.OrderItems!.OrderId == request.OrderId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(r => r.RequestedDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(r => r.RequestedDate <= request.ToDate.Value);
        }

        if (request.OnGoing.HasValue)
        {
            if (request.OnGoing.Value == true)
            {
                query = query.Where(r => r.ReturnStatusId == 2 || (r.ReturnStatusId>=4  && r.ReturnStatusId<=8) || r.ReturnStatusId == 11);
            }
        }

        var totalCount = await query.CountAsync();

        var data = await query.OrderByDescending(r => r.RequestedDate).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (data, totalCount);
    }

    public async Task<(List<Return> data, int totalCount)> GetAllReturnsForVendor(RequestVendorReturnFilter request, int vendorId)
    {
        var query = BaseQuery().Where(r => r.OrderItems!.ProductVariant!.Product!.VendorId == vendorId);

        if (request.ReturnStatusId.HasValue)
        {
            query = query.Where(r => r.ReturnStatusId == request.ReturnStatusId.Value);
        }
        if (request.ReturnReasonId.HasValue)
        {
            query = query.Where(r => r.ReturnReasonId == request.ReturnReasonId.Value);
        }
        if (request.OrderItemId.HasValue)
        {
            query = query.Where(r => r.OrderItemId == request.OrderItemId.Value);
        }
        if (request.OrderId.HasValue)
        {
            query = query.Where(r => r.OrderItems!.OrderId == request.OrderId.Value);
        }
        if (request.FromDate.HasValue)
        {
            query = query.Where(r => r.RequestedDate >= request.FromDate.Value);
        }
        if (request.ToDate.HasValue)
        {
            query = query.Where(r => r.RequestedDate <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync();

        var data = await query.OrderByDescending(r => r.RequestedDate).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();

        return (data, totalCount);
    }

    public async Task<(List<Return> data, int totalCount)> GetAllReturnsForUser(RequestUserReturnFilter request, int userId)
    {
        var query = BaseQuery().Where(r => r.OrderItems!.Order!.UserId == userId);
        if (request.ReturnStatusId.HasValue)
        {
            query = query.Where(r => r.ReturnStatusId == request.ReturnStatusId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(r => r.RequestedDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(r => r.RequestedDate <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync();

        var data = await query.OrderByDescending(r => r.RequestedDate).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (data, totalCount);
    }

    public async Task<Return?> GetReturnDetails(int returnId)
    {
        return await _ecommerceContext.Return
            .Include(r => r.ReturnReason)
            .Include(r => r.ReturnStatus)
            .Include(r => r.ReviewedByVendor)
                .ThenInclude(v => v!.User)
            .Include(r => r.OrderItems)
                .ThenInclude(oi => oi!.Order)
                    .ThenInclude(o => o!.Users)
            .Include(r => r.OrderItems)
                .ThenInclude(oi => oi!.Order)
                    .ThenInclude(o => o!.OrderStatus)
            .Include(r => r.OrderItems)
                .ThenInclude(oi => oi!.Order)
                    .ThenInclude(o => o!.Address)
            .Include(r => r.OrderItems)
                .ThenInclude(oi => oi!.ProductVariant)
                    .ThenInclude(pv => pv!.Product)
            .Include(r => r.OrderItems)
                .ThenInclude(oi => oi!.ProductVariant)
                    .ThenInclude(pv => pv!.ProductImages)
            .Include(r => r.OrderItems)
                .ThenInclude(oi => oi!.ProductVariant)
                    .ThenInclude(pv => pv!.ProductVariantAttributes)
                        .ThenInclude(a => a.ProductSubCategoryAttribute)
                            .ThenInclude(v => v!.AttributeMaster)
            .FirstOrDefaultAsync(r => r.ReturnId == returnId);
    }

}