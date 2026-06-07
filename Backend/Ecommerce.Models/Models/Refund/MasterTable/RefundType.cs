using Ecommerce.Models;

public class RefundType
{
    public int RefundTypeId {get;set;}
    public string RefundTypeName {get;set;} = string.Empty;
    public ICollection<Refund> Refunds {get;set;} = new List<Refund>();
}