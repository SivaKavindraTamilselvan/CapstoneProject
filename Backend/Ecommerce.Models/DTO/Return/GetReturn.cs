using Ecommerce.DTOs;

public class ReturnSummaryDto
{
    public int ReturnId { get; set; }
    public string ProductImageUrl { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public int ReturnQuantity { get; set; }
    public decimal ReturnAmount { get; set; }
    public string ReturnReason { get; set; } = string.Empty;
    public string ReturnStatus { get; set; } = string.Empty;
    public string InventoryCity { get; set; } = string.Empty;
    public string DeliveryCity { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public DateTime? ReviewedDate { get; set; }
}

public class RequestAdminReturnFilter : PaginationFilter
{
    public int? ReturnStatusId { get; set; }
    public int? ReturnReasonId { get; set; }
    public int? VendorId { get; set; }
    public int? OrderItemId { get; set; }
    public int? OrderId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class RequestVendorReturnFilter : PaginationFilter
{
    public int? ReturnStatusId { get; set; }
    public int? ReturnReasonId { get; set; }
    public int? OrderItemId { get; set; }
    public int? OrderId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class RequestUserReturnFilter : PaginationFilter
{
    public int? ReturnStatusId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}