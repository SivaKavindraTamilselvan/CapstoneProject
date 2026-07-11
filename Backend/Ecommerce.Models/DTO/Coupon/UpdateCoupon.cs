namespace Ecommerce.DTOs;

public class UpdateCouponDto
{
    public int CouponId {get;set;}
    public string? CouponDescription { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MinimumNumberOfUsage { get; set; }
    public bool? IsActive { get; set; }
}