namespace Ecommerce.Models;
public class CouponType
{
    public int CouponTypeId {get;set;}
    public string CouponTypeName {get;set;} = string.Empty;
    public ICollection<Coupons> Coupons {get;set;} = new List<Coupons>();
}