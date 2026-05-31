namespace Ecommerce.Models;

public class Refund
{
    public int RefundId {get;set;}
    public int OrderId {get;set;}
    public Order? Order {get;set;}
    public int RefundStatusId {get;set;}
    public RefundStatus? RefundStatus {get;set;}
    public DateTime RequestedDate {get;set;}
    public DateTime? ProcessedDate {get;set;}
    public ICollection<RefundItems> RefundItems {get;set;} = new List<RefundItems>();
    public ICollection<Payment> Payments {get;set;} = new List<Payment>();
}