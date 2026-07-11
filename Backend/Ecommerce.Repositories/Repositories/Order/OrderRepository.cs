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
                .ThenInclude(oi => oi.ProductVariant)
                    .ThenInclude(pv => pv!.Product)
                        .ThenInclude(p => p!.ProductImages)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderItemStatus)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Inventory)
                .ThenInclude(i => i!.Address)
            .Include(o => o.OrderItems).ThenInclude(o => o.Returns).ThenInclude(r => r.ReturnReason)
            .Include(o => o.OrderItems).ThenInclude(o => o.Returns).ThenInclude(r => r.ReturnStatus)
            .AsNoTracking();
    }

    // get order by order id in user and admin
    public async Task<Order?> GetOrderByOrderId(int orderId)
    {
        var query = GetBaseQuery();
        query = query.Where(p => p.OrderId == orderId);
        return await query.FirstOrDefaultAsync();
    }

    // get admin orders
    public async Task<(List<Order> items, int totalCount)> GetOrdersForAdmin(AdminOrderFilterParams filters)
    {
        var query = GetBaseQuery();
        if (!string.IsNullOrWhiteSpace(filters.OrderNumber))
            query = query.Where(o => o.OrderNumber.Contains(filters.OrderNumber));

        if (filters.FromDate.HasValue)
            query = query.Where(o => o.OrderDate >= filters.FromDate.Value);

        if (filters.ToDate.HasValue)
            query = query.Where(o => o.OrderDate <= filters.ToDate.Value.AddDays(1));

        if (filters.OrderStatusId.HasValue)
            query = query.Where(o => o.OrderStatusId == filters.OrderStatusId.Value);

        if (filters.MinAmount.HasValue)
            query = query.Where(o => o.FinalAmount >= filters.MinAmount.Value);

        if (filters.MaxAmount.HasValue)
            query = query.Where(o => o.FinalAmount <= filters.MaxAmount.Value);

        if (filters.UserId.HasValue)
            query = query.Where(o => o.UserId == filters.UserId.Value);

        if (filters.VendorId.HasValue)
            query = query.Where(o =>
                o.OrderItems.Any(oi =>
                    oi.ProductVariant!.Product!.VendorId == filters.VendorId.Value));

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedAt).Skip((filters.PageNumber - 1) * filters.PageSize).Take(filters.PageSize).ToListAsync();
        return (items, totalCount);
    }

    // get user orders
    public async Task<(List<Order> items, int totalCount)> GetOrdersForUser(int userId, OrderFilterParams filters)
    {
        var query = GetBaseQuery().Where(o => o.UserId == userId);
        query = query.Where(o => o.OrderStatusId != 1);

        if (!string.IsNullOrWhiteSpace(filters.OrderNumber))
            query = query.Where(o => o.OrderNumber.Contains(filters.OrderNumber));

        if (filters.FromDate.HasValue)
            query = query.Where(o => o.OrderDate >= filters.FromDate.Value);

        if (filters.ToDate.HasValue)
            query = query.Where(o => o.OrderDate <= filters.ToDate.Value.AddDays(1));

        if (filters.OrderStatusId.HasValue)
            query = query.Where(o => o.OrderStatusId == filters.OrderStatusId.Value);

        if (filters.MinAmount.HasValue)
            query = query.Where(o => o.FinalAmount >= filters.MinAmount.Value);

        if (filters.MaxAmount.HasValue)
            query = query.Where(o => o.FinalAmount <= filters.MaxAmount.Value);

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedAt).Skip((filters.PageNumber - 1) * filters.PageSize).Take(filters.PageSize).ToListAsync();
        return (items, totalCount);

    }

    public async Task<OrderItems?> GetOrderItemsByOrderItemId(int orderItemId)
    {
        var query = _ecommerceContext.OrderItems
    .Include(oi => oi.Order)

    .Include(oi => oi.ProductVariant)
        .ThenInclude(pv => pv!.Product)
            .ThenInclude(p => p!.ProductImages)

    .Include(oi => oi.ProductVariant)
        .ThenInclude(pv => pv!.Product)
            .ThenInclude(p => p!.Vendor)

    .Include(oi => oi.Inventory)
        .ThenInclude(i => i!.Address)

    .Include(oi => oi.OrderItemStatus)
    .Include(o => o.Returns).ThenInclude(r => r.ReturnReason)
            .Include(o => o.Returns).ThenInclude(r => r.ReturnStatus)

    .Where(oi => oi.OrderItemsId == orderItemId);
        return await query.FirstOrDefaultAsync();

    }



    // get vendor orders
    public async Task<(List<OrderItems> items, int totalCount)> GetOrderItemsForVendor(int vendorId, OrderFilterParams filters)
    {
        var query = _ecommerceContext.OrderItems
       .Include(oi => oi.Order)
       .Include(oi => oi.ProductVariant)
           .ThenInclude(pv => pv!.Product)
       .Include(oi => oi.Inventory)
           .ThenInclude(i => i!.Address)
       .Include(oi => oi.OrderItemStatus)
       .Where(oi =>
           oi.ProductVariant != null &&
           oi.ProductVariant.Product != null &&
           oi.ProductVariant.Product.VendorId == vendorId
       );


        if (filters.FromDate.HasValue)
            query = query.Where(o => o.Order!.OrderDate >= filters.FromDate.Value);

        if (filters.ToDate.HasValue)
            query = query.Where(o => o.Order!.OrderDate <= filters.ToDate.Value.AddDays(1));

        if (filters.OrderStatusId.HasValue)
            query = query.Where(o => o.Order!.OrderStatusId == filters.OrderStatusId.Value);

        if (filters.OrderItemStatusId.HasValue)
            query = query.Where(o => o.OrderItemStatusId == filters.OrderItemStatusId.Value);


        if (filters.MinAmount.HasValue)
            query = query.Where(o => o.Order!.FinalAmount >= filters.MinAmount.Value);

        if (filters.MaxAmount.HasValue)
            query = query.Where(o => o.Order!.FinalAmount <= filters.MaxAmount.Value);


        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.Order!.CreatedAt).Skip((filters.PageNumber - 1) * filters.PageSize).Take(filters.PageSize).ToListAsync();
        return (items, totalCount);
    }
    public async Task<decimal> GetGrossSalesAsync()
    {
        return await _ecommerceContext.OrderItems
            .Where(oi => oi.Order!.OrderStatusId == 2 &&
                         oi.OrderItemStatusId == 4)
            .SumAsync(oi => oi.UnitPrice * oi.Quantity);
    }

    public async Task<decimal> GetCommissionAsync()
    {
        return await _ecommerceContext.OrderItems
            .Where(oi => oi.Order!.OrderStatusId == 2 &&
                         oi.OrderItemStatusId == 4)
            .SumAsync(oi =>
                oi.ProductVariant!.Product!.ProductSubCategory!.CommissionPercentage
                * oi.UnitPrice
                * oi.Quantity / 100m);
    }

    public async Task<decimal> GetYesterdayCommissionAsync()
    {
        var start = DateTime.Today.AddDays(-1);
        var end = start.AddDays(1);

        return await _ecommerceContext.OrderItems
            .Where(oi => oi.Order!.OrderStatusId == 2 &&
                         oi.OrderItemStatusId == 4 &&
                         oi.Order.OrderDate >= start &&
                         oi.Order.OrderDate < end)
            .SumAsync(oi =>
                oi.ProductVariant!.Product!.ProductSubCategory!.CommissionPercentage
                * oi.UnitPrice
                * oi.Quantity / 100m);
    }
    public async Task<decimal> GetLast30DaysCommissionAsync()
    {
        var fromDate = DateTime.Today.AddDays(-30);

        return await _ecommerceContext.OrderItems
            .Where(oi => oi.Order!.OrderStatusId == 2 &&
                         oi.OrderItemStatusId == 4 &&
                         oi.Order.OrderDate >= fromDate)
            .SumAsync(oi =>
                oi.ProductVariant!.Product!.ProductSubCategory!.CommissionPercentage
                * oi.UnitPrice
                * oi.Quantity / 100m);
    }
    public async Task<int> GetTotalOrdersAsync()
    {
        return await _ecommerceContext.Order.CountAsync();
    }

    public async Task<int> GetPendingOrdersAsync()
    {
        return await _ecommerceContext.Order
            .CountAsync(o => o.OrderStatusId == 1);
    }

    public async Task<int> GetTotalCustomersAsync()
    {
        return await _ecommerceContext.User
            .CountAsync(u => u.RoleId == 2);
    }

    public async Task<int> GetActiveVendorsAsync()
    {
        return await _ecommerceContext.Vendor
            .CountAsync(v => v.IsActive);
    }

    public async Task<List<RevenueTrendDto>> GetRevenueTrendAsync(string period)
    {
        var query = _ecommerceContext.OrderItems
            .Where(oi => oi.Order!.OrderStatusId == 2 &&
                         oi.OrderItemStatusId == 4);

        if (period == "7days")
        {
            var fromDate = DateTime.Today.AddDays(-6);

            var data = await query
                .Where(oi => oi.Order!.OrderDate >= fromDate)
                .GroupBy(oi => oi.Order!.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data.Select(x => new RevenueTrendDto
            {
                Label = x.Date.ToString("ddd"),
                Revenue = x.Revenue
            }).ToList();
        }

        if (period == "30days")
        {
            var fromDate = DateTime.Today.AddDays(-29);

            var data = await query
                .Where(oi => oi.Order!.OrderDate >= fromDate)
                .GroupBy(oi => oi.Order!.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data.Select(x => new RevenueTrendDto
            {
                Label = x.Date.ToString("dd MMM"),
                Revenue = x.Revenue
            }).ToList();
        }

        var fromMonth = DateTime.Today.AddMonths(-11);

        var monthlyData = await query
            .Where(oi => oi.Order!.OrderDate >= fromMonth)
            .GroupBy(oi => new
            {
                oi.Order!.OrderDate.Year,
                oi.Order.OrderDate.Month
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        return monthlyData.Select(x => new RevenueTrendDto
        {
            Label = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
            Revenue = x.Revenue
        }).ToList();
    }
    public async Task<List<ProductApprovalStatusDto>> GetProductApprovalStatusAsync()
    {
        return await _ecommerceContext.Product
            .GroupBy(p => p.ProductApprovalStatus!.ProductApprovalStatusName)
            .Select(g => new ProductApprovalStatusDto
            {
                Status = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Status)
            .ToListAsync();
    }
    public async Task<List<ProductSubCategoryDto>> GetProductsPerSubCategoryAsync()
    {
        return await _ecommerceContext.Product
            .GroupBy(p => p.ProductSubCategory!.ProductSubCategoryName)
            .Select(g => new ProductSubCategoryDto
            {
                SubCategory = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();
    }
    public async Task<List<OrderStatusChartDto>> GetOrdersByStatusAsync()
    {
        return await _ecommerceContext.Order
            .GroupBy(o => o.OrderStatus!.OrderStatusName)
            .Select(g => new OrderStatusChartDto
            {
                Status = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Status)
            .ToListAsync();
    }
    public async Task<List<OrdersByMonthDto>> GetOrdersByMonthAsync()
    {
        var fromDate = DateTime.Today.AddMonths(-11);

        var data = await _ecommerceContext.Order
            .Where(o => o.OrderDate >= fromDate)
            .GroupBy(o => new
            {
                o.OrderDate.Year,
                o.OrderDate.Month
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        return data.Select(x => new OrdersByMonthDto
        {
            Month = new DateTime(x.Year, x.Month, 1).ToString("MMM"),
            Count = x.Count
        }).ToList();
    }

    #region Dashboard Cards

    public async Task<decimal> GetVendorGrossSalesAsync(int vendorId)
    {
        return await _ecommerceContext.OrderItems
            .Where(oi =>
                oi.Order!.OrderStatusId == 2 &&
                oi.OrderItemStatusId == 4 &&
                oi.ProductVariant!.Product!.VendorId == vendorId)
            .SumAsync(oi => oi.UnitPrice * oi.Quantity);
    }

    public async Task<decimal> GetVendorYesterdaySalesAsync(int vendorId)
    {
        var start = DateTime.Today.AddDays(-1);
        var end = start.AddDays(1);

        return await _ecommerceContext.OrderItems
            .Where(oi =>
                oi.Order!.OrderStatusId == 2 &&
                oi.OrderItemStatusId == 4 &&
                oi.ProductVariant!.Product!.VendorId == vendorId &&
                oi.Order.OrderDate >= start &&
                oi.Order.OrderDate < end)
            .SumAsync(oi => oi.UnitPrice * oi.Quantity);
    }

    public async Task<decimal> GetVendorLast30DaysSalesAsync(int vendorId)
    {
        var fromDate = DateTime.Today.AddDays(-30);

        return await _ecommerceContext.OrderItems
            .Where(oi =>
                oi.Order!.OrderStatusId == 2 &&
                oi.OrderItemStatusId == 4 &&
                oi.ProductVariant!.Product!.VendorId == vendorId &&
                oi.Order.OrderDate >= fromDate)
            .SumAsync(oi => oi.UnitPrice * oi.Quantity);
    }

    public async Task<int> GetVendorTotalOrdersAsync(int vendorId)
    {
        return await _ecommerceContext.OrderItems
            .Where(oi => oi.ProductVariant!.Product!.VendorId == vendorId)
            .Select(oi => oi.OrderId)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> GetVendorPendingOrdersAsync(int vendorId)
    {
        return await _ecommerceContext.OrderItems
            .Where(oi =>
                oi.ProductVariant!.Product!.VendorId == vendorId &&
                oi.Order!.OrderStatusId == 1)
            .Select(oi => oi.OrderId)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> GetVendorTotalProductsAsync(int vendorId)
    {
        return await _ecommerceContext.Product
            .CountAsync(p => p.VendorId == vendorId);
    }

    public async Task<int> GetVendorActiveProductsAsync(int vendorId)
    {
        return await _ecommerceContext.Product
            .CountAsync(p =>
                p.VendorId == vendorId &&
                p.ProductStatusId == 1);
    }

    public async Task<int> GetVendorLowStockProductsAsync(int vendorId)
    {
        return await _ecommerceContext.ProductVariant
            .CountAsync(v =>
                v.Product!.VendorId == vendorId &&
                v.Inventories.Sum(i => i.AvailableQuantity) <= 10);
    }
    #endregion

    #region Revenue Trend

    public async Task<List<RevenueTrendDto>> GetVendorRevenueTrendAsync(int vendorId, string period)
    {
        var query = _ecommerceContext.OrderItems
            .Where(oi =>
                oi.ProductVariant!.Product!.VendorId == vendorId &&
                oi.Order!.OrderStatusId == 2 &&
                oi.OrderItemStatusId == 4);

        if (period == "7days")
        {
            var fromDate = DateTime.Today.AddDays(-6);

            var data = await query
                .Where(oi => oi.Order!.OrderDate >= fromDate)
                .GroupBy(oi => oi.Order!.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data.Select(x => new RevenueTrendDto
            {
                Label = x.Date.ToString("ddd"),
                Revenue = x.Revenue
            }).ToList();
        }

        if (period == "30days")
        {
            var fromDate = DateTime.Today.AddDays(-29);

            var data = await query
                .Where(oi => oi.Order!.OrderDate >= fromDate)
                .GroupBy(oi => oi.Order!.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data.Select(x => new RevenueTrendDto
            {
                Label = x.Date.ToString("dd MMM"),
                Revenue = x.Revenue
            }).ToList();
        }

        var fromMonth = DateTime.Today.AddMonths(-11);

        var monthlyData = await query
            .Where(oi => oi.Order!.OrderDate >= fromMonth)
            .GroupBy(oi => new
            {
                oi.Order!.OrderDate.Year,
                oi.Order.OrderDate.Month
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        return monthlyData.Select(x => new RevenueTrendDto
        {
            Label = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
            Revenue = x.Revenue
        }).ToList();
    }

    #endregion

    #region Charts

    public async Task<List<ProductApprovalStatusDto>> GetVendorProductApprovalStatusAsync(int vendorId)
    {
        return await _ecommerceContext.Product
            .Where(p => p.VendorId == vendorId)
            .GroupBy(p => p.ProductApprovalStatus!.ProductApprovalStatusName)
            .Select(g => new ProductApprovalStatusDto
            {
                Status = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Status)
            .ToListAsync();
    }

    public async Task<List<ProductSubCategoryDto>> GetVendorProductsPerSubCategoryAsync(int vendorId)
    {
        return await _ecommerceContext.Product
            .Where(p => p.VendorId == vendorId)
            .GroupBy(p => p.ProductSubCategory!.ProductSubCategoryName)
            .Select(g => new ProductSubCategoryDto
            {
                SubCategory = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();
    }

    public async Task<List<OrderStatusChartDto>> GetVendorOrdersByStatusAsync(int vendorId)
    {
        return await _ecommerceContext.OrderItems
            .Where(oi => oi.ProductVariant!.Product!.VendorId == vendorId)
            .GroupBy(oi => oi.Order!.OrderStatus!.OrderStatusName)
            .Select(g => new OrderStatusChartDto
            {
                Status = g.Key,
                Count = g.Select(x => x.OrderId).Distinct().Count()
            })
            .OrderBy(x => x.Status)
            .ToListAsync();
    }

    public async Task<List<OrdersByMonthDto>> GetVendorOrdersByMonthAsync(int vendorId)
    {
        var fromDate = DateTime.Today.AddMonths(-11);

        var data = await _ecommerceContext.OrderItems
            .Where(oi =>
                oi.ProductVariant!.Product!.VendorId == vendorId &&
                oi.Order!.OrderDate >= fromDate)
            .GroupBy(oi => new
            {
                oi.Order!.OrderDate.Year,
                oi.Order.OrderDate.Month
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Count = g.Select(x => x.OrderId).Distinct().Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        return data.Select(x => new OrdersByMonthDto
        {
            Month = new DateTime(x.Year, x.Month, 1).ToString("MMM"),
            Count = x.Count
        }).ToList();
    }

    #endregion
}