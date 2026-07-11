using Ecommerce.Data;
using Ecommerce.DTOs.Shipment;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ShipmentRepsository : AbstractRepository<int, Shipment>, IShipmentRepsository
{
    public ShipmentRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<Shipment?> GetShipmentByOrderItemId(int orderitemid)
    {
        var shipment = await _ecommerceContext.Shipment.Include(s => s.ShipmentItems).Include(s => s!.ShipmentStatus).Where(s => s.ShipmentItems.Any(si => si.OrderItemsId == orderitemid)).FirstOrDefaultAsync();
        return shipment;
    }
    public async Task<List<Shipment>> GetShipmentByOrderId(int orderid)
    {
        var shipment = await _ecommerceContext.Shipment.Include(s => s.ShipmentItems).Include(s => s!.ShipmentStatus).Where(s => s.OrderId == orderid).ToListAsync();
        return shipment;
    }
    private IQueryable<Shipment> BaseQuery()
    {
        return _ecommerceContext.Shipment.Include(s => s.ShipmentStatus)
        .Include(s => s.PickupAddress)
        .Include(s => s.Order).ThenInclude(o => o!.Users)
        .Include(s => s.ShipmentItems).ThenInclude(si => si.OrderItems).ThenInclude(oi => oi!.ProductVariant).ThenInclude(pv => pv!.Product)
        .Include(s => s.ShipmentTrackings).ThenInclude(st => st.ShipmentStatus).AsNoTracking();
    }

    public async Task<(List<Shipment> Items, int TotalCount)> GetAllShipmentsForAdmin(RequestShipmentFilter filter)
    {
        var query = BaseQuery();

        if (filter.ShipmentStatusId.HasValue)
        {
            query = query.Where(s => s.ShipmentStatusId == filter.ShipmentStatusId.Value);
        }
        if (filter.ShipmentTypeId.HasValue)
        {
            query = query.Where(s => s.ShipmentTypeId == filter.ShipmentTypeId.Value);
        }
        if (filter.PickUpAddressId.HasValue)
        {
            query = query.Where(s => s.ShipmentTypeId == filter.PickUpAddressId.Value);
        }
        if (filter.OrderId.HasValue)
        {
            query = query.Where(s => s.OrderId == filter.OrderId.Value);
        }
        if (!string.IsNullOrWhiteSpace(filter.TrackingNumber))
        {
            query = query.Where(s => s.TrackingNumber != null && s.TrackingNumber.Contains(filter.TrackingNumber));
        }
        if (filter.FromDate.HasValue)
        {
            query = query.Where(s => s.CreatedAt >= filter.FromDate.Value);
        }
        if (filter.ToDate.HasValue)
        {
            query = query.Where(s => s.CreatedAt <= filter.ToDate.Value.Date.AddDays(1));
        }
        if (!string.IsNullOrWhiteSpace(filter.CourierName))
        {
            query = query.Where(s => s.CourierName.ToLower() == filter.CourierName.ToLower());
        }
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(s => s.CreatedAt).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<Shipment?> GetShipmentDetailForAdmin(int shipmentId)
    {
        return await BaseQuery().FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);
    }

    public async Task<Shipment?> GetShipmentDetailForOrderItemsId(int orderItemsId)
    {
        return await BaseQuery().FirstOrDefaultAsync(s => s.Order!.OrderItems.Any(o => o.OrderItemsId == orderItemsId));
    }

}