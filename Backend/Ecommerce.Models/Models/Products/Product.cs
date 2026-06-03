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
    public int ProductApprovalStatusId {get;set;} = 1;
    public ProductApprovalStatus? ProductApprovalStatus {get;set;}
    public int AddedByVendorUserId {get;set;}
    public VendorUser? AddedByVendorUser {get;set;}
    public int ProductStatusId {get;set;} = 1;
    public ProductStatus? ProductStatus {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime? UpdatedAt{get;set;}
    public ICollection<ProductVariant> ProductVariants {get;set;} = new List<ProductVariant>();
    public ICollection<ProductImage> ProductImages {get;set;} = new List<ProductImage>();
    public ICollection<CouponsProduct> CouponsProducts {get;set;} = new List<CouponsProduct>();
    public ICollection<ApprovalHistory> ApprovalHistories {get;set;} = new List<ApprovalHistory>();
}