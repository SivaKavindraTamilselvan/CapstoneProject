public class RequestRefundCancelDTO
{
    public int CancelId { get; set; }
    public int RefundTypeId {get;set;}
    public int OrderItemId { get; set; }
}

public class ResponseRefundCancelDTO
{
    public int RefundId {get;set;}
    public decimal? ActualRefundAmount {get;set;}
    public DateTime RequestedDate {get;set;}
    public DateTime? ProcessedDate {get;set;}
}