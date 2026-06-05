namespace Ecommerce.Models;

public class ResponseGetOrderItems
{
    public int OrderItemsId { get; set; }
    public int OrderId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public int OrderItemStatusId { get; set; }
    public int InventoryId { get; set; }
}