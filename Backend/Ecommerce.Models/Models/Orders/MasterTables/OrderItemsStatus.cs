namespace Ecommerce.Models;
public class OrderItemStatus
{
    public int OrderItemStatusId {get;set;}
    public string OrderItemStatusName {get;set;} = string.Empty;
    public ICollection<OrderItems> OrderItems {get;set;} = new List<OrderItems>();
}