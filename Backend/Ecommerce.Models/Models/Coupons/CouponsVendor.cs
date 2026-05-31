namespace Ecommerce.Models;

public class CouponsVendor
{
    public int CouponsVendorId {get;set;}
    public int CouponId {get;set;}
    public Coupons? Coupons {get;set;}
    public int VendorId {get;set;}
    public Vendor? Vendor {get;set;}
}