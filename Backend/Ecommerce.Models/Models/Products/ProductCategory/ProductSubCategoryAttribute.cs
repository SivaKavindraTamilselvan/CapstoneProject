namespace Ecommerce.Models;

public class ProductSubCategoryAttribute
{
    public int ProductSubCategoryAttributeId { get; set; }
    public int ProductSubCategoryId { get; set; }
    public ProductSubCategory? ProductSubCategory { get; set; }
    public int AttributeMasterId { get; set; }
    public AttributeMaster? AttributeMaster { get; set; }
    public bool IsActive {get;set;} = true;
    public int AddedByAdminId {get;set;}
    public AdminUser? AddedByAdminUser {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public ICollection<ProductVariant> ProductVariants {get;set;} = new List<ProductVariant>();
    public ICollection<ProductVariantAttribute> ProductVariantAttributes { get; set; } = new List<ProductVariantAttribute>();

}