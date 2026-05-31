namespace Ecommerce.Models;

public class RefundItems
{
    public int RefundItemsId {get;set;}
    public int OrderItemsId {get;set;}
    public OrderItems? OrderItems {get;set;}
    public int RefundId {get;set;}
    public Refund? Refund{get;set;}
    public decimal RefundAmount {get;set;}
    public int RefundQuantity {get;set;}
}