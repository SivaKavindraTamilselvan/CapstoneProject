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

    // get the orders that are active in inventory to avoid the address deletion
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
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Inventory)
                .ThenInclude(i => i!.Address)
            .AsNoTracking();
    }

    // get order by order id in user and admin
    public async Task<Order?> GetOrderByOrderId(int orderId)
    {
        var query = GetBaseQuery();
        query = query.Where(p=>p.OrderId == orderId);
        return await query.FirstOrDefaultAsync();
    }

    // get admin orders
    public async Task<List<Order>> GetOrdersForAdmin(AdminOrderFilterParams filters,OrderFilterParams orderFilterParams)
    {
        var query = GetBaseQuery();
        query = ApplyCommonFilters(query, orderFilterParams);
        if (filters.UserId.HasValue)
            query = query.Where(o => o.UserId == filters.UserId.Value);

        if (filters.VendorId.HasValue)
            query = query.Where(o =>
                o.OrderItems.Any(oi =>
                    oi.ProductVariant!.Product!.VendorId == filters.VendorId.Value));
        return await query.OrderByDescending(c=>c.CreatedAt).Skip((filters.Page - 1) * filters.PageSize).Take(filters.PageSize).ToListAsync();
    }

    // get user orders
    public async Task<List<Order>> GetOrdersForUser(int userId, OrderFilterParams filters)
    {
        var query = GetBaseQuery().Where(o => o.UserId == userId);

        query = ApplyCommonFilters(query, filters);

        return await query.OrderByDescending(c=>c.CreatedAt).Skip((filters.Page - 1) * filters.PageSize).Take(filters.PageSize).ToListAsync();
    }

    // get vendor orders
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