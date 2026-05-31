namespace Ecommerce.Models;

public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public byte[] Password { get; set; } = [];
    public byte[] HashedKey { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public int RoleId { get; set; }
    public Role? Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public Cart? Cart {get;set;}
    public Favorites? Favorites {get;set;}
    public AdminUser? AdminUsers {get;set;}
    public VendorUser? VendorUser {get;set;}
    public ICollection<Order> Orders {get;set;} = new List<Order>();
    public ICollection<Address> Addresses {get;set;} = new List<Address>();
    public ICollection<LogChanges> LogChanges {get;set;} = new List<LogChanges>();
}