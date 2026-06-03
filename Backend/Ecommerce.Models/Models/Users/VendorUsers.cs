namespace Ecommerce.Models;

public class VendorUser
{
    public int VendorUserId { get; set; }
    public int VendorId { get; set; }
    public Vendor? Vendor { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int VendorRoleId { get; set; }
    public VendorRole? VendorRole { get; set; }
    public int? AddedByVendorUserId { get; set; }
    public VendorUser? AddedByVendor { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Product> Products {get;set;} = new List<Product>();
    public ICollection<ProductVariant> ProductVariants {get;set;} = new List<ProductVariant>();
    public ICollection<ProductImage> ProductImages {get;set;} = new List<ProductImage>();
}