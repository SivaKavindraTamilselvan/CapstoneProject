namespace Ecommerce.Models;

public class Return
{
    public int ReturnId {get;set;}
    public int ReturnReasonId {get;set;}
    public ReturnReason? ReturnReason {get;set;}
    public int OrderId {get;set;}
    public Order? Order {get;set;}
    public int ReturnStatusId {get;set;}
    public ReturnStatus? ReturnStatus {get;set;}
    public string? AdditionalReason {get;set;}
    public DateTime RequestedDate {get;set;}
    public int ReviewedByAdminId {get;set;}
    public AdminUser? ReviewedByAdmin {get;set;}
    public DateTime? ReviewedDate {get;set;}
    public ICollection<ReturnItems> ReturnItems {get;set;} = new List<ReturnItems>();
}