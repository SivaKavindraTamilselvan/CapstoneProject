namespace Ecommerce.Models;

public class CouponsProduct
{
    public int CouponsProductId {get;set;}
    public int CouponId {get;set;}
    public Coupons? Coupons {get;set;}
    public int ProductId {get;set;}
    public Product? Product {get;set;}
}