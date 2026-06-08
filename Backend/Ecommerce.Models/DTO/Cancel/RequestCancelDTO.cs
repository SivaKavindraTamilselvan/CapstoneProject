public class RequestCancelDTO
{
    public int CancelReasonId { get; set; }
    public int OrderItemId { get; set; }
    public int CancelStatusId { get; set; }
    public string? AdditionalReason { get; set; }
    public int CancelQuantity { get; set; }
}

public class ResponseCancelDTO
{
    public int CancelId { get; set; }
    public DateTime CancelledDate { get; set; }
    public decimal ConvenienceFee { get; set; }
}