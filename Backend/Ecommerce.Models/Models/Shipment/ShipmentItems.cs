namespace Ecommerce.Models;
public class ShipmentItems
{
    public int ShipmentItemsId {get;set;}
    public int ShipmentId {get;set;}
    public Shipment? Shipment {get;set;}
    public int OrderIemsId {get;set;}
    public OrderItems? OrderItems {get;set;}
}