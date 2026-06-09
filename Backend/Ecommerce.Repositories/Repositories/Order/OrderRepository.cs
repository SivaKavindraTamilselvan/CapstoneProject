using System.Data;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class OrderRepsository : AbstractRepository<int, Order> ,IOrderRepsository
{
    public OrderRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<List<Order>> GetOrdersByUserId(int? status,int pageNumber,int pageSize,int userId)
    {
        var order = _ecommerceContext.Order.Include(o=>o.OrderItems).ThenInclude(o=>o.OrderItemStatus).Include(o=>o.OrderStatus).Include(a=>a.Address)
        .Include(o=>o.OrderItems).ThenInclude(o=>o.ProductVariant).ThenInclude(o=>o!.Product).Include(c=>c.CouponUsages).Where(u=>u.UserId == userId);
        return await order.OrderByDescending(o=>o.CreatedAt).Skip((pageNumber - 1)*pageSize).Take(pageSize).ToListAsync();
    }
    public async Task<List<Order>> GetPendingOrdersByAddress(int address)
    {
        var order = _ecommerceContext.Order.Where(o=>o.AddressId == address && (o.OrderStatusId == 1 || o.OrderStatusId == 2));
        return await order.ToListAsync();
    }
}