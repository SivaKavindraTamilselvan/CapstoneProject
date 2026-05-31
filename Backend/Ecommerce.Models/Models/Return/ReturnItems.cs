namespace Ecommerce.Models;
public class ReturnItems
{
    public int ReturnItemsId {get;set;}
    public int ReturnId {get;set;}
    public Return? Return {get;set;}
    public int OrderItemsId {get;set;}
    public OrderItems? OrderItems {get;set;}
    public int ReturnQuantity {get;set;}
}