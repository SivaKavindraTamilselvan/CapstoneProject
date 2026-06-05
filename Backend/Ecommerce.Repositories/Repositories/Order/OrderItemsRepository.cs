using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class OrderItemRepsository : AbstractRepository<int, OrderItems> ,IOrderItemRepsository
{
    public OrderItemRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    public async Task<List<OrderItems>> GetOrderItemsByVendor(int vendorId)
    {
        var orders = await _ecommerceContext.OrderItems.Include(o=>o.ProductVariant).ThenInclude(p=>p.Product).Include(i=>i.Inventory).Where(p=>p.ProductVariant.Product.VendorId == vendorId && p.OrderItemStatusId == 1).ToListAsync();
        return orders;
    }
    
}