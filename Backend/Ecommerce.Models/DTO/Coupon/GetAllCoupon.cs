namespace Ecommerce.DTOs;

public class ResponseGetAllCoupon()
{
    public int CouponId { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MinimumNumberOfUsage { get; set; }
}

public class AdminCouponFilter : PaginationFilter
{
    public int? CouponId { get; set; }
    public string? Search { get; set; }
    public int? CouponTypeId { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsExpired { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public decimal? MinDiscountValue { get; set; }
    public decimal? MaxDiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxOrderAmount { get; set; }
}

public class CouponListDto
{
    public int CouponId { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MinimumNumberOfUsage { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
    public string CouponTypeName { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CouponUsageDto
{
    public int CouponUsageId { get; set; }
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal OrderFinalAmount { get; set; }
    public DateTime UsedAt { get; set; }
}

public class CouponDetailDto
{
    public int CouponId { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public string CouponDescription { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MinimumNumberOfUsage { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
    public int CouponTypeId { get; set; }
    public string CouponTypeName { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UsageCount { get; set; }
    public List<int> ApplicableProductIds { get; set; } = new();
    public List<CouponUsageDto> UsageHistory { get; set; } = new();
}
