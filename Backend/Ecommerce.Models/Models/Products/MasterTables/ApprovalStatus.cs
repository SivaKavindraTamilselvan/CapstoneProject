namespace Ecommerce.Models;
public class ApprovalStatus
{
    public int ApprovalStatusId {get;set;}
    public string ApprovalStatusName {get;set;} = string.Empty;
    public ICollection<Vendor> Vendors {get;set;} = new List<Vendor>();
}