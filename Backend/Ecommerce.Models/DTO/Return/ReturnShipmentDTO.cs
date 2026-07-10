namespace Ecommerce.DTOs;
public class ResponseCreateReturnShipmentDTO
{
    public int ShipmentId { get; set; }
    public int OrderId { get; set; }
    public decimal ShippingCharge { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string TrackingRemarks { get; set; } = string.Empty;
}