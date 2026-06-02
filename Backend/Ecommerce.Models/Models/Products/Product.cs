namespace Ecommerce.Models;
public class Product
{
    public int ProductId {get;set;}
    public int VendorId {get;set;}
    public Vendor? Vendor {get;set;}
    public string ProductName {get;set;} = string.Empty;
    public int ProductSubCategoryId {get;set;}
    public ProductSubCategory? ProductSubCategory {get;set;}
    public string Description {get;set;} = string.Empty;
    public int ApprovalStatusId {get;set;} = 1;
    public ApprovalStatus? ApprovalStatus {get;set;}
    public int? ReviewedByAdminId {get;set;}
    public AdminUser? ReviewedByAdmin {get;set;}
    public int ProductStatusId {get;set;} = 1;
    public ProductStatus? ProductStatus {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime? ApprovedAt {get;set;}
    public ICollection<ProductVariant> ProductVariants {get;set;} = new List<ProductVariant>();
    public ICollection<ProductImage> ProductImages {get;set;} = new List<ProductImage>();
    public ICollection<CouponsProduct> CouponsProducts {get;set;} = new List<CouponsProduct>();
}