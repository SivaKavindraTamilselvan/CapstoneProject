namespace Ecommerce.Models;
public class ModeOfPayment
{
    public int ModeOfPaymentId {get;set;}
    public string ModeOfPaymentName {get;set;} = string.Empty;
    public ICollection<Payment> Payments {get;set;} = new List<Payment>();
}