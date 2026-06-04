namespace Ecommerce.DTOs;

public class RequestAddCouponDTO
{
    public string CouponCode {get;set;} = string.Empty;
    public decimal DiscountValue {get;set;}
    public decimal MinimumOrderAmount {get;set;}
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    public int MinimumNumberOfUsage {get;set;}
    public string CouponDescription {get;set;} = string.Empty;
}

public class RequestAddCouponProductDTO
{
    public int CouponId {get;set;}
    public int ProductId {get;set;}
}

public class ResponseAddCouponProductDTO
{
    public int CouponsProductId {get;set;}
    public DateTime CreatedAt {get;set;}
}

public class ResponseAddCouponDTO
{
    public int CouponId {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;

}