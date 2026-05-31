namespace Ecommerce.Models;

public class ReturnReason
{
    public int ReturnReasonId {get;set;}
    public string ReturnReasonDescription {get;set;} = string.Empty;
    public ICollection<Return> Returns {get;set;} = new List<Return>();
}