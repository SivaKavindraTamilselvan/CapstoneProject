namespace Ecommerce.Models;

public class ReturnStatus
{
    public int ReturnStatusId {get;set;}
    public string ReturnStatusName {get;set;} = string.Empty;
    public ICollection<Return> Returns {get;set;} = new List<Return>();
}