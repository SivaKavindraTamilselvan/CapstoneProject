
namespace Ecommerce.DTOs.Shipment;

public class ShipmentFilterDto
{
    public int? ShipmentStatusId { get; set; }
    public int? OrderId { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ShipmentTrackingResponseDto
{
    public int ShipmentTrackingId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ShipmentItemResponseDto
{
    public int ShipmentItemsId { get; set; }
    public int OrderItemsId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class ShipmentDetailResponseDto
{
    public int ShipmentId { get; set; }
    public int OrderId { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
    public decimal ShippingCharge { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public int? ShipperId { get; set; }
    public string ShipperName { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;

    public List<ShipmentItemResponseDto> Items { get; set; } = new();
    public List<ShipmentTrackingResponseDto> Tracking { get; set; } = new();
}

public class ShipmentSummaryResponseDto
{
    public int ShipmentId { get; set; }
    public int OrderId { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
    public decimal ShippingCharge { get; set; }
    public string ShipperName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime ExpectedDeliveryDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalItems { get; set; }
}