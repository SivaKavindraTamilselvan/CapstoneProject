namespace Ecommerce.Models;

public class Shipment
{
    public int ShipmentId { get; set; }
    public int? ShipperId { get; set; }
    public Shipper? Shipper { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int PickupAddressId { get; set; }
    public Address? PickupAddress { get; set; }

    public string? TrackingNumber { get; set; }
    public decimal ShippingCharge { get; set; }
    public int ShipmentStatusId { get; set; }
    public ShipmentStatus? ShipmentStatus { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<ShipmentItems> ShipmentItems { get; set; } = new List<ShipmentItems>();
    public ICollection<ShipmentTracking> ShipmentTrackings { get; set; } = new List<ShipmentTracking>();
}