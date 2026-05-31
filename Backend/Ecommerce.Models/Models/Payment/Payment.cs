namespace Ecommerce.Models;
public class Payment
{
    public int PaymentId {get;set;}
    public int OrderId {get;set;}
    public Order? Order {get;set;}
    public int? RefundId{get;set;}
    public Refund? Refund {get;set;}
    public int ModeOfPaymentId {get;set;}
    public ModeOfPayment? ModeOfPayment {get;set;}
    public string PaymentGatewayOrderId {get;set;} = string.Empty;
    public string PaymentGatewayTransactionId {get;set;} = string.Empty;
    public int PaymentStatusId {get;set;}
    public PaymentStatus? PaymentStatus {get;set;}
    public DateTime? PaymentDate {get;set;}
    public string? FailureReason {get;set;}
    public decimal Amount {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime? UpdatedAt {get;set;}
}