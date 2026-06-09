using System.Data;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class OrderRepsository : AbstractRepository<int, Order>, IOrderRepsository
{
    public OrderRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<List<Order>> GetOrdersByUserId(int? status, int pageNumber, int pageSize, int userId)
    {
        var order = _ecommerceContext.Order.Include(o => o.OrderItems).ThenInclude(o => o.OrderItemStatus).Include(o => o.OrderStatus).Include(a => a.Address)
        .Include(o => o.OrderItems).ThenInclude(o => o.ProductVariant).ThenInclude(o => o!.Product).Include(c => c.CouponUsages).Where(u => u.UserId == userId);
        return await order.OrderByDescending(o => o.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }
    public async Task<List<Order>> GetPendingOrdersByAddress(int address)
    {
        var order = _ecommerceContext.Order.Where(o => o.AddressId == address && (o.OrderStatusId == 1 || o.OrderStatusId == 2));
        return await order.ToListAsync();
    }

    public IQueryable<Order> GetBaseQuery()
    {
        return _ecommerceContext.Order
            .Include(o => o.Users)
            .Include(o => o.OrderStatus)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                    .ThenInclude(pv => pv!.Product)
                        .ThenInclude(p => p!.Vendor)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderItemStatus)
            .AsNoTracking();
    }
    public async Task<List<Order>> GetOrdersForAdmin(OrderFilterParams filters)
    {
        var query = GetBaseQuery();
        query = ApplyCommonFilters(query, filters);
        if (filters.UserId.HasValue)
            query = query.Where(o => o.UserId == filters.UserId.Value);

        if (filters.VendorId.HasValue)
            query = query.Where(o =>
                o.OrderItems.Any(oi =>
                    oi.ProductVariant!.Product!.VendorId == filters.VendorId.Value));
        return await query.OrderByDescending(c=>c.CreatedAt).Skip((filters.Page - 1) * filters.PageSize).Take(filters.PageSize).ToListAsync();
    }
    public async Task<List<Order>> GetOrdersForUser(int userId, OrderFilterParams filters)
    {
        var query = GetBaseQuery().Where(o => o.UserId == userId);

        query = ApplyCommonFilters(query, filters);

        return await query.OrderByDescending(c=>c.CreatedAt).Skip((filters.Page - 1) * filters.PageSize).Take(filters.PageSize).ToListAsync();
    }
    public async Task<List<Order>> GetOrdersForVendor(int vendorId, OrderFilterParams filters)
    {

        var query = GetBaseQuery().Where(o => o.OrderItems.Any(oi =>oi.ProductVariant!.Product!.VendorId == vendorId));
        query = ApplyCommonFilters(query, filters);

        return await query.OrderByDescending(c=>c.CreatedAt).Skip((filters.Page - 1) * filters.PageSize).Take(filters.PageSize).ToListAsync();
    }
    private static IQueryable<Order> ApplyCommonFilters(IQueryable<Order> query, OrderFilterParams f)
    {
        if (!string.IsNullOrWhiteSpace(f.OrderNumber))
            query = query.Where(o => o.OrderNumber.Contains(f.OrderNumber));

        if (f.FromDate.HasValue)
            query = query.Where(o => o.OrderDate >= f.FromDate.Value);

        if (f.ToDate.HasValue)
            query = query.Where(o => o.OrderDate <= f.ToDate.Value.AddDays(1));

        if (f.OrderStatusId.HasValue)
            query = query.Where(o => o.OrderStatusId == f.OrderStatusId.Value);

        if (f.MinAmount.HasValue)
            query = query.Where(o => o.FinalAmount >= f.MinAmount.Value);

        if (f.MaxAmount.HasValue)
            query = query.Where(o => o.FinalAmount <= f.MaxAmount.Value);

        return query.OrderByDescending(o => o.OrderDate);
    }
}