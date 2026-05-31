namespace Ecommerce.Models;

public class Vendor
{
    public int VendorId { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public string CompanyPhoneNumber { get; set; } = string.Empty;
    public string VendorCompanyName { get; set; } = string.Empty;
    public string GSTNumber { get; set; } = string.Empty;
    public int ApprovalStatusId { get; set; }
    public ApprovalStatus? ApprovalStatus { get; set; }
    public int? ReviewedByAdminId { get; set; }
    public AdminUser? ReviwedByAdmin { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ReviewedAt { get; set; }
    public ICollection<VendorUser> VendorUsers { get; set; } = new List<VendorUser>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<CouponsVendor> CouponsVendors { get; set; } = new List<CouponsVendor>();
}