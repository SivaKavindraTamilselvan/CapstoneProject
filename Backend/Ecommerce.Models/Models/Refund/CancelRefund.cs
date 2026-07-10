
namespace Ecommerce.Models;
public class CancelRefund
{
    public int CancelRefundId { get; set; }
    public int RefundId { get; set; }
    public Refund? Refund { get; set; }
    public int CancelId { get; set; }
    public Cancel? Cancel { get; set; }
}