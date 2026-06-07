namespace Ecommerce.Models;

public class OrderItems
{
    public int OrderItemsId { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public int OrderItemStatusId { get; set; }
    public int InventoryId {get;set;}
    public Inventory? Inventory {get;set;}
    public OrderItemStatus? OrderItemStatus { get; set; }
    public ICollection<Return> Returns { get; set; } = new List<Return>();
    public ICollection<Refund> Refunds {get;set;} = new List<Refund>();
    public ICollection<ShipmentItems> ShipmentItems { get; set; } = new List<ShipmentItems>();
    public Reviews? Reviews { get; set; }
}