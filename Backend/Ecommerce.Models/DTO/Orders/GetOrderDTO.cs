// OrderFilterParams.cs
public class OrderFilterParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
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
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public string OrderItemStatus { get; set; } = string.Empty;
}

// PagedResult.cs
public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}