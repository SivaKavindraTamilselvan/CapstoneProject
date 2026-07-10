namespace Ecommerce.Models;

public class ShipmentStatus
{
    public int ShipmentStatusId {get;set;}
    public string ShipmentStatusName {get;set;} = string.Empty;
    public ICollection<Shipment> Shipments {get;set;} = new List<Shipment>();
    public ICollection<ShipmentTracking> ShipmentTrackings { get; set; } = new List<ShipmentTracking>();
}