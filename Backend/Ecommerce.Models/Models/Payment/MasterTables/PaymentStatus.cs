namespace Ecommerce.Models;
public class PaymentStatus
{
    public int PaymentStatusId {get;set;}
    public string PaymentStatusName {get;set;} = string.Empty;
    public ICollection<Payment> Payments {get;set;} = new List<Payment>();
}