namespace Ecommerce.Models;
public class ShipmentTracking
{
    public int ShipmentTrackingId {get;set;}
    public int ShipmentId {get;set;}
    public Shipment? Shipment {get;set;}
    public int ShipmentStatusId {get;set;}
    public ShipmentStatus? ShipmentStatus {get;set;}
    public string Location {get;set;} = string.Empty;
    public string? Remarks {get;set;} = string.Empty;
    public DateTime? UpdatedAt {get;set;}
}