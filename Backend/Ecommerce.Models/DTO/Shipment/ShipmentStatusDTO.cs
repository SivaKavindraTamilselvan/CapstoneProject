namespace Ecommerce.DTOs;

public class ShipmentStatusRequestDTO
{
    public int ShipmentId { get; set; }
    public int ShipmentStatusId { get; set; }
}

public class ShipmentStatusResponseDTO
{
    public int ShipmentId { get; set; }
    public int OrderId { get; set; }
    public int PickupAddressId { get; set; }
    public string? TrackingNumber { get; set; }
    public decimal ShippingCharge { get; set; }
    public int ShipmentStatusId { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}