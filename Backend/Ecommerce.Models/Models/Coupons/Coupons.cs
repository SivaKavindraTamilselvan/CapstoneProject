namespace Ecommerce.Models;
public class Coupons
{
    public int CouponId {get;set;}
    public string CouponCode {get;set;} = string.Empty;
    public decimal DiscountValue {get;set;}
    public decimal MinimumOrderAmount {get;set;}
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    public int MinimumNumberOfUsage {get;set;}
    public bool IsActive {get;set;} = true;
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public ICollection<CouponsProduct> CouponsProducts {get;set;} = new List<CouponsProduct>();
    public ICollection<CouponsVendor> CouponsVendors {get;set;} = new List<CouponsVendor>();
    public ICollection<CouponUsage> CouponUsages {get;set;} = new List<CouponUsage>();
}