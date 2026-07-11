// OrderFilterParams.cs
using Ecommerce.DTOs;

public class AdminOrderFilterParams : PaginationFilter
{
    public string? OrderNumber { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? OrderStatusId { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }

    // Admin-only filters
    public int? UserId { get; set; }
    public int? VendorId { get; set; }
}

public class OrderFilterParams : PaginationFilter
{
    public string? OrderNumber { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? OrderStatusId { get; set; }
    public int? OrderItemStatusId { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

// OrderSummaryDto.cs
public class OrderSummaryDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal TotalProductAmount { get; set; }
    public decimal TotalShippingAmount { get; set; }
    public decimal TotalCouponAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public int TotalItems { get; set; }
    public List<OrderItemSummaryDto> OrderItems { get; set; } = new();
}

// OrderItemSummaryDto.cs
public class OrderItemSummaryDto
{
    public int OrderItemsId { get; set; }
    public string ProductImageUrl { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public int InventoryId { get; set; }
    public bool canReturn { get; set; }
    public List<ReturnSummaryDto> returns { get; set; }
    public string InventoryCity { get; set; } = string.Empty;
    public string InventoryAddress { get; set; } = string.Empty;
    public decimal ItemTotal { get; set; }
    public string OrderItemStatus { get; set; } = string.Empty;
}