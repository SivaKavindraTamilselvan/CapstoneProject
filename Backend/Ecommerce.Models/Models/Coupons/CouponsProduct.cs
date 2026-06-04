namespace Ecommerce.Models;

public class CouponsProduct
{
    public int CouponsProductId { get; set; }
    public int CouponId { get; set; }
    public Coupons? Coupons { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int AddedByVendorUserId { get; set; }
    public VendorUser? AddedByVendorUser { get; set; }
}