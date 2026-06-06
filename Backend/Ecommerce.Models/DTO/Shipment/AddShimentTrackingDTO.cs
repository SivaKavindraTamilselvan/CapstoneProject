namespace Ecommerce.DTOs;

public class RequestAddShipmentTrackingDTO
{
    public int ShipmentId { get; set; }
    public int ShipmentStatusId { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Remarks { get; set; } = string.Empty;
}
public class ResponseAddShipmentTrackingDTO
{
    public int ShipmentTrackingId { get; set; }
    public DateTime? UpdatedAt { get; set; }
}