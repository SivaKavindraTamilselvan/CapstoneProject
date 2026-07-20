using System.IO.Pipes;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class OrderItemRepsository : AbstractRepository<int, OrderItems>, IOrderItemRepsository
{
    public OrderItemRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<OrderItems?> GetOrderItemByOrderItemId(int orderItemId)
    {
        var order = _ecommerceContext.OrderItems.Include(o => o.Order).Include(o => o.ProductVariant).Where(o => o.OrderItemsId == orderItemId);
        return await order.FirstOrDefaultAsync();
    }

    public async Task<List<OrderItems>> GetOrderItemByOrderId(int orderId)
    {
        var order = _ecommerceContext.OrderItems.Include(p => p.ProductVariant).ThenInclude(p => p.Product).Include(o => o.ProductVariant).Where(o => o.OrderId == orderId);
        return await order.ToListAsync();
    }

    // for vendor valid order
    public async Task<List<OrderItems>> GetOrderItemsByVendor(int vendorId, int? status)
    {
        var orders = _ecommerceContext.OrderItems.Include(o => o.OrderItemStatus).Include(o => o.ProductVariant).ThenInclude(p => p!.Product).ThenInclude(p => p!.Vendor).Include(i => i!.Inventory).ThenInclude(a => a!.Address).Where(p => p.ProductVariant!.Product!.VendorId == vendorId);
        if (status.HasValue)
        {
            orders = orders.Where(p => p.OrderItemStatusId == status.Value);
        }
        return await orders.ToListAsync();
    }
    public async Task<List<OrderItems>> GetAllOrderItemsByVendor(int vendorId, int? status)
    {
        var orders = _ecommerceContext.OrderItems.Include(o => o.OrderItemStatus).Include(o => o.ProductVariant).ThenInclude(p => p!.Product).Include(i => i.Inventory).Where(p => p.ProductVariant!.Product!.VendorId == vendorId);
        if (status.HasValue)
        {
            orders.Where(o => o.OrderItemStatusId == status.Value);
        }
        return await orders.ToListAsync();
    }
    public async Task<List<OrderItems>> GetCancelledOrderItemsByOrderId(int orderId)
    {
        return await _ecommerceContext.OrderItems.Where(o => o.OrderId == orderId && o.OrderItemStatusId != 7).ToListAsync();
    }
    /*
    public async Task<List<OrderItems>> GetOrderByInventoryAddress(int? status, int vendorId, int pageNumber, int pageSize, int addressId)
    {
        var query = _ecommerceContext.OrderItems.Include(oi => oi.Order).Include(oi => oi.OrderItemStatus).Include(oi => oi.Inventory).ThenInclude(i => i!.Address).Include(oi => oi.ProductVariant)
        .ThenInclude(pv => pv!.Product).Where(oi => oi.ProductVariant!.Product!.VendorId == vendorId && oi.Inventory!.AddressId == addressId);

        if (status.HasValue)
        {
            query = query.Where(oi => oi.OrderItemStatusId == status.Value);
        }

        return await query.OrderByDescending(oi => oi.Order!.OrderDate).ThenByDescending(oi => oi.OrderItemsId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }
    */

    // get order for particular inventory address
    public async Task<List<OrderItems>> GetPendingOrderByInventoryAddress(int addressId)
    {
        return await _ecommerceContext.OrderItems.Include(o => o.Inventory).Where(a => a.Inventory!.AddressId == addressId && a.OrderItemStatusId != 1 && a.OrderItemStatusId == 2 && a.OrderItemStatusId == 3 && a.OrderItemStatusId == 5).ToListAsync();
    }

    public async Task<decimal> GetCommissionAsync()
    {
        return await _ecommerceContext.OrderItems.Where(oi => oi.Order!.OrderStatusId == 2 && oi.OrderItemStatusId == 4).SumAsync(oi => oi.ProductVariant!.Product!.ProductSubCategory!.CommissionPercentage * oi.UnitPrice * oi.Quantity / 100m);
    }

}