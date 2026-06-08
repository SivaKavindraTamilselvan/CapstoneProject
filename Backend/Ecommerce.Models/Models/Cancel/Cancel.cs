namespace Ecommerce.Models;

public class Cancel
{
    public int CancelId { get; set; }
    public int CancelReasonId { get; set; }
    public CancelReason? CancelReason { get; set; }
    public int OrderItemId { get; set; }
    public OrderItems? OrderItems { get; set; }
    public int CancelStatusId { get; set; }
    public CancelStatus? CancelStatus { get; set; }
    public string? AdditionalReason { get; set; }
    public DateTime CancelledDate { get; set; }
    public int CancelQuantity { get; set; }
    public decimal ConvenienceFee { get; set; }
    public CancelRefund? CancelRefund { get; set; }
}