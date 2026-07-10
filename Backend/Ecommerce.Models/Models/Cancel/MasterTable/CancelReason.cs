namespace Ecommerce.Models;

public class CancelReason
{
    public int CancelReasonId {get;set;}
    public string CancelReasonDescription {get;set;} = string.Empty;
    public ICollection<Cancel> Cancels {get;set;} = new List<Cancel>();
}