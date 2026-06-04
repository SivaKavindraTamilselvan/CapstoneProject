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
    public string CouponDescription {get;set;} = string.Empty;

    public int? CreatedByAdminUserId { get; set; }
    public AdminUser? CreatedByAdminUser { get; set; }
    public int? CreatedByVendorUserId { get; set; }
    public VendorUser? CreatedByVendorUser { get; set; }
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime? UpdatedAt {get;set;}
    public ICollection<CouponsProduct> CouponsProducts {get;set;} = new List<CouponsProduct>();
    public ICollection<CouponUsage> CouponUsages {get;set;} = new List<CouponUsage>();
}