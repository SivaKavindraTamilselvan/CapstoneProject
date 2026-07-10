namespace Ecommerce.Models;

public class RefundStatus
{
    public int RefundStatusId {get;set;}
    
    public string RefundStatusName {get;set;} = string.Empty;
    public ICollection<Refund> Refunds {get;set;} = new List<Refund>();
}