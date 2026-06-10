using Ecommerce.Data;
using Ecommerce.DTOs.Shipment;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ShipmentItemRepsository : AbstractRepository<int, ShipmentItems>, IShipmentItemsRepsository
{
    public ShipmentItemRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<List<ShipmentItems>> GetTheShipmentItemsByShipmentId(int shipmentId)
    {
        var shipment = await _ecommerceContext.ShipmentItems.Where(s => s.ShipmentId == shipmentId).ToListAsync();
        return shipment;
    }
    public async Task<List<ShipmentItems>> GetPendingPackedTheShipmentItemsByShipmentId(int shipmentId)
    {
        var shipment = await _ecommerceContext.ShipmentItems.Include(s => s.OrderItems).Where(s => s.ShipmentId == shipmentId && s.OrderItems.OrderItemStatusId != 2).ToListAsync();
        return shipment;
    }
    public async Task<List<OrderItems>> GetOrderItemsByShippingId(int shipmentId)
    {
        var orderItem = await _ecommerceContext.ShipmentItems.Where(s => s.ShipmentId == shipmentId).Select(s => s.OrderItems!).ToListAsync();
        return orderItem;
    }
    public async Task<List<Shipment>> GetAllNotDeliveredOrderByOrderId(int orderId)
    {
        var shipment = await _ecommerceContext.Shipment.Where(s => s.OrderId == orderId && s.ShipmentStatusId != 8).ToListAsync();
        return shipment;
    }

    private IQueryable<Shipment> BaseQuery()
    {
        return _ecommerceContext.Shipment
            .Include(s => s.ShipmentStatus)
            .Include(s => s.Shipper)
            .Include(s => s.PickupAddress)
            .Include(s => s.Order)
                .ThenInclude(o => o!.Users)
            .Include(s => s.ShipmentItems)
                .ThenInclude(si => si.OrderItems)
                    .ThenInclude(oi => oi!.ProductVariant)
            .Include(s => s.ShipmentTrackings)
                .ThenInclude(st => st.ShipmentStatus);
    }
    public async Task<(List<ShipmentSummaryResponseDto> Items, int TotalCount)> GetAllShipmentsForAdmin(
        ShipmentFilterDto filter)
    {
        var query = BaseQuery();

        if (filter.ShipmentStatusId.HasValue)
            query = query.Where(s => s.ShipmentStatusId == filter.ShipmentStatusId);

        if (filter.ShipperId.HasValue)
            query = query.Where(s => s.ShipperId == filter.ShipperId);

        if (filter.OrderId.HasValue)
            query = query.Where(s => s.OrderId == filter.OrderId);

        if (!string.IsNullOrWhiteSpace(filter.TrackingNumber))
            query = query.Where(s => s.TrackingNumber!.Contains(filter.TrackingNumber));

        if (filter.FromDate.HasValue)
            query = query.Where(s => s.CreatedAt >= filter.FromDate);
        if (filter.ToDate.HasValue)
            query = query.Where(s => s.CreatedAt <= filter.ToDate.Value.Date.AddDays(1));

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(s => new ShipmentSummaryResponseDto
            {
                ShipmentId = s.ShipmentId,
                OrderId = s.OrderId,
                CurrentStatus = s.ShipmentStatus!.ShipmentStatusName,
                TrackingNumber = s.TrackingNumber,
                ShippingCharge = s.ShippingCharge,
                ShipperName = "Unassigned",
                CustomerName = s.Order!.Users!.FirstName + " " + s.Order.Users.LastName,
                ExpectedDeliveryDate = s.ExpectedDeliveryDate,
                ShippedDate = s.ShippedDate,
                DeliveryDate = s.DeliveryDate,
                CreatedAt = s.CreatedAt,
                TotalItems = s.ShipmentItems.Count
            })
            .ToListAsync();
        return (items, total);
    }
    public async Task<ShipmentDetailResponseDto?> GetShipmentDetailForAdmin(int shipmentId)
    {
        var shipment = await BaseQuery()
            .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

        if (shipment == null) return null;

        return MapToDetail(shipment);
    }
    public async Task<List<ShipmentSummaryResponseDto>> GetShipmentsByOrderForAdmin(int orderId)
    {
        return await BaseQuery()
            .Where(s => s.OrderId == orderId)
            .OrderBy(s => s.CreatedAt)
            .Select(s => new ShipmentSummaryResponseDto
            {
                ShipmentId = s.ShipmentId,
                OrderId = s.OrderId,
                CurrentStatus = s.ShipmentStatus!.ShipmentStatusName,
                TrackingNumber = s.TrackingNumber,
                ShippingCharge = s.ShippingCharge,
                ShipperName = "Unassigned",
                CustomerName = s.Order!.Users!.FirstName + " " + s.Order.Users.LastName,
                ExpectedDeliveryDate = s.ExpectedDeliveryDate,
                ShippedDate = s.ShippedDate,
                DeliveryDate = s.DeliveryDate,
                CreatedAt = s.CreatedAt,
                TotalItems = s.ShipmentItems.Count
            })
            .ToListAsync();
    }
    public async Task<ShipmentDetailResponseDto?> GetShipmentByTrackingForAdmin(string trackingNumber)
    {
        var shipment = await BaseQuery()
            .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);

        if (shipment == null) return null;

        return MapToDetail(shipment);
    }
    private static ShipmentDetailResponseDto MapToDetail(Shipment s)
    {
        return new ShipmentDetailResponseDto
        {
            ShipmentId = s.ShipmentId,
            OrderId = s.OrderId,
            CurrentStatus = s.ShipmentStatus!.ShipmentStatusName,
            TrackingNumber = s.TrackingNumber,
            ShippingCharge = s.ShippingCharge,
            ExpectedDeliveryDate = s.ExpectedDeliveryDate,
            ShippedDate = s.ShippedDate,
            DeliveryDate = s.DeliveryDate,
            CreatedAt = s.CreatedAt,

            ShipperId = s.ShipperId,
            ShipperName = "Unassigned",

            PickupAddress = s.PickupAddress != null
                ? $"{s.PickupAddress.AddressLine}, {s.PickupAddress.City}, {s.PickupAddress.State} - {s.PickupAddress.PinCode}"
                : string.Empty,

            CustomerName = s.Order?.Users != null
                ? $"{s.Order.Users.FirstName} {s.Order.Users.LastName}"
                : string.Empty,
            CustomerEmail = s.Order?.Users?.Email ?? string.Empty,
            Items = s.ShipmentItems.Select(si => new ShipmentItemResponseDto
            {
                ShipmentItemsId = si.ShipmentItemsId,
                OrderItemsId = si.OrderItemsId,
                ProductName = si.OrderItems?.ProductVariant?.Product?.ProductName ?? string.Empty,
                SKU = si.OrderItems?.ProductVariant?.SKU ?? string.Empty,
                Quantity = si.OrderItems?.Quantity ?? 0,
                UnitPrice = si.OrderItems?.UnitPrice ?? 0
            }).ToList(),

            Tracking = s.ShipmentTrackings
                .OrderByDescending(t => t.UpdatedAt)
                .Select(t => new ShipmentTrackingResponseDto
                {
                    ShipmentTrackingId = t.ShipmentTrackingId,
                    Status = t.ShipmentStatus?.ShipmentStatusName ?? string.Empty,
                    Location = t.Location,
                    Remarks = t.Remarks,
                    UpdatedAt = t.UpdatedAt
                }).ToList()
        };
    }

}