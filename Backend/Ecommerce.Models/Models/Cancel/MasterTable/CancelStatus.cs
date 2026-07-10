namespace Ecommerce.Models;

public class CancelStatus
{
    public int CancelStatusId {get;set;}
    public string CancelStatusName {get;set;} = string.Empty;
    public ICollection<Cancel> Cancels {get;set;} = new List<Cancel>();
}