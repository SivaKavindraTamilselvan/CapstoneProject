namespace Ecommerce.Models;
public class CouponUsage
{
    public int CouponUsageId {get;set;}
    public int CouponId {get;set;}
    public Coupons? Coupons {get;set;}
    public int OrderId {get;set;}
    public Order? Order {get;set;}
    public DateTime UsedAt {get;set;}
}