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

    public async Task<List<OrderItems>> GetOrderItemsByVendor(int vendorId)
    {
        var orders = await _ecommerceContext.OrderItems.Include(o => o.ProductVariant).ThenInclude(p => p.Product).Include(i => i.Inventory).Where(p => p.ProductVariant.Product.VendorId == vendorId && p.OrderItemStatusId == 1).ToListAsync();
        return orders;
    }
    public async Task<List<OrderItems>> GetOrderItemsByOrderId(int orderId)
    {
        return await _ecommerceContext.OrderItems.Include(o => o.Order).Where(o => o.OrderId == orderId).ToListAsync();
    }
    public async Task<List<OrderItems>> GetCancelledOrderItemsByOrderId(int orderId)
    {
        return await _ecommerceContext.OrderItems.Where(o => o.OrderId == orderId && o.OrderItemStatusId == 7).ToListAsync();
    }
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
    public async Task<List<OrderItems>> GetPendingOrderByInventoryAddress(int addressId)
    {
        return await _ecommerceContext.OrderItems.Include(o => o.Inventory).Where(a => a.Inventory!.AddressId == addressId && a.OrderItemStatusId != 1 && a.OrderItemStatusId == 2 && a.OrderItemStatusId == 3 && a.OrderItemStatusId == 5).ToListAsync();
    }
}