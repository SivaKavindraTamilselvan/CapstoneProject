namespace Ecommerce.Models;
public class OrderStatus
{
    public int OrderStatusId {get;set;}
    public string OrderStatusName {get;set;} = string.Empty;
    public ICollection<Order> Orders {get;set;} = new List<Order>();
}