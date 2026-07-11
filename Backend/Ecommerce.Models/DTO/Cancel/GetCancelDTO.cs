using Ecommerce.DTOs;

public class CancelSummaryDto
{
    public int CancelId { get; set; }
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;

    public int OrderedQuantity { get; set; }
    public int CancelQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal ConvenienceFee { get; set; }
    public decimal CancelAmount { get; set; }

    public string CancelReason { get; set; } = string.Empty;
    public string CancelStatus { get; set; } = string.Empty;
    public string? AdditionalReason { get; set; }

    public DateTime CancelledDate { get; set; }

    public string DeliveryCity { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryPincode { get; set; } = string.Empty;
    public int UserId {get;set;}
    public string ContactPersonName {get;set;} = string.Empty;
    public string ContactPhoneNumber {get;set;} = string.Empty;

}

public class RequestAdminCancelFilter : PaginationFilter
{
    public int? CancelStatusId { get; set; }
    public int? CancelReasonId { get; set; }
    public int? VendorId { get; set; }
    public int? OrderId { get; set; }
    public int? OrderItemId { get; set; }
    public int? ProductVariantId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
public class RequestVendorCancelFilter : PaginationFilter
{
    public int? CancelStatusId { get; set; }
    public int? CancelReasonId { get; set; }
    public int? OrderId { get; set; }
    public int? ProductVariantId { get; set; }
    public int? OrderItemId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class RequestUserCancelFilter : PaginationFilter
{
    public int? CancelStatusId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class CancelReasonResponse
{
    public int CancelReasonId {get;set;}
    public string CancelReasonDescription {get;set;} = string.Empty;
}