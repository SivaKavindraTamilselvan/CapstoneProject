namespace Ecommerce.DTOs;

public class RequestAddShipmentDTO
{
    public int OrderId { get; set; }
    public string CourierName {get;set;} = string.Empty;
    public int PickupAddressId { get; set; }
    public decimal ShippingCharge { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }

}

public class ResponseAddShipmentDTO
{
    public int ShipmentId { get; set; }
    public DateTime CreatedAt { get; set; }
}