namespace Ecommerce.Models;
public class AdminUser
{
    public int AdminUserId {get;set;}
    public int UserId {get;set;}
    public User? User {get;set;}
    public int AdminRoleId {get;set;}
    public AdminRole? AdminRole {get;set;}
    public int? AssignedByAdminUserId {get;set;}
    public AdminUser? AssignedByAdmin {get;set;}
    public bool IsActive {get;set;} = true;
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public ICollection<ProductCategory> ProductCategories {get;set;} = new List<ProductCategory>();
    public ICollection<ProductSubCategory> ProductSubCategories {get;set;} = new List<ProductSubCategory>();
    public ICollection<ProductSubCategoryAttribute> ProductSubCategoryAttributes {get;set;} = new List<ProductSubCategoryAttribute>();
    public ICollection<AttributeMaster> AttributeMasters {get;set;} = new List<AttributeMaster>();
    public ICollection<Shipper> Shippers {get;set;} = new List<Shipper>();
    public ICollection<Vendor> Vendors {get;set;} = new List<Vendor>();

}