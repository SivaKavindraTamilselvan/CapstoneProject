namespace Ecommerce.Models;

public class ShipmentType
{
    public int ShipmentTypeId {get;set;}
    public string ShipmentTypeName {get;set;} = string.Empty;
    public ICollection<Shipment> Shipments {get;set;} = new List<Shipment>();
}