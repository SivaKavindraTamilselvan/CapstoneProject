namespace Ecommerce.DTOs;

public class ResponseGetAllCoupon()
{
    public int CouponId {get;set;}
    public string CouponCode {get;set;} = string.Empty;
    public decimal DiscountValue {get;set;}
    public decimal MinimumOrderAmount {get;set;}
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    public int MinimumNumberOfUsage {get;set;}
}