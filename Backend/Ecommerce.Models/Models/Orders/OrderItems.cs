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
    public OrderItemStatus? OrderItemStatus { get; set; }
    public ICollection<RefundItems> RefundItems { get; set; } = new List<RefundItems>();
    public ICollection<ReturnItems> ReturnItems { get; set; } = new List<ReturnItems>();
    public ICollection<ShipmentItems> ShipmentItems { get; set; } = new List<ShipmentItems>();
    public Reviews? Reviews { get; set; }
}