namespace Ecommerce.Models;
public class Shipper
{
    public int ShipperId {get;set;}
    public string CompanyName {get;set;} = string.Empty;
    public string APIBaseURL {get;set;} = string.Empty;
    public int CreatedByAdminId {get;set;}
    public AdminUser? CreatedByAdmin {get;set;}
    public bool IsActive {get;set;} = true;
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public ICollection<Shipment> Shipments {get;set;} = new List<Shipment>();
}