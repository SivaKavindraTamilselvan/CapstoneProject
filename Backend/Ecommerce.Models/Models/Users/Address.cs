namespace Ecommerce.Models;
public class Address
{
    public int AddressId {get;set;}
    public int UserId {get;set;}
    public User? User {get;set;}
    public string ContactName {get;set;} = string.Empty; // for deliver 
    public string ContactPhoneNumber {get;set;} = string.Empty; // for delivery
    public string AddressLine {get;set;} = string.Empty;
    public string LandMark {get;set;} = string.Empty;
    public string City {get;set;} = string.Empty;
    public string State {get;set;} = string.Empty;
    public string Country {get;set;} = string.Empty;
    public string PinCode {get;set;} = string.Empty;
    public bool IsDefault {get;set;} = false;
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime? UpdatedAt {get;set;}
    public ICollection<Order> Orders {get;set;} = new List<Order>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}