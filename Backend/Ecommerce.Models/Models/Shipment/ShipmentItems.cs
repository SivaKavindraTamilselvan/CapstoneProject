namespace Ecommerce.Models;
public class ShipmentItems
{
    public int ShipmentItemsId {get;set;}
    public int ShipmentId {get;set;}
    public Shipment? Shipment {get;set;}
    public int OrderItemsId {get;set;}
    public OrderItems? OrderItems {get;set;}
}