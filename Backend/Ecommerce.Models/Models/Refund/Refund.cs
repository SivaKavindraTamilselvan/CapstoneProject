namespace Ecommerce.Models;

public class Refund
{
    public int RefundId { get; set; }
    public int RefundTypeId { get; set; }
    public RefundType? RefundType { get; set; }
    public int OrderItemsId { get; set; }
    public OrderItems? OrderItems { get; set; }
    public int RefundStatusId { get; set; }
    public RefundStatus? RefundStatus { get; set; }
    public decimal? ActualRefundAmount { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public ReturnRefund? ReturnRefund { get; set; }
    public CancelRefund? CancelRefund { get; set; }
    public decimal RefundAmount { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}