
namespace Ecommerce.Models;
public class ReturnRefund
{
    public int ReturnRefundId { get; set; }
    public int RefundId { get; set; }
    public Refund? Refund { get; set; }
    public int ReturnId { get; set; }
    public Return? Return { get; set; }
    public decimal DamageCost { get; set; }
    public string DeductionReason { get; set; } = string.Empty;
}