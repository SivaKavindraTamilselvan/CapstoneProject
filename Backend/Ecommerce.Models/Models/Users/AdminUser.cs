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
    public ICollection<Product> Products {get;set;} = new List<Product>();
    public ICollection<Shipper> Shippers {get;set;} = new List<Shipper>();
    public ICollection<Vendor> Vendors {get;set;} = new List<Vendor>();

}