namespace Ecommerce.Models;

public class Return
{
    public int ReturnId {get;set;}
    public int ReturnReasonId {get;set;}
    public ReturnReason? ReturnReason {get;set;}
    public int OrderItemId {get;set;}
    public OrderItems? OrderItems {get;set;}
    public int ReturnStatusId {get;set;}
    public ReturnStatus? ReturnStatus {get;set;}
    public string? AdditionalReason {get;set;}
    public DateTime RequestedDate {get;set;}
    public int ReturnQuantity {get;set;}
    public decimal? DamageCost {get;set;}
    public decimal?  ConvenienceFee {get;set;}
    public string? ReviewRemarks {get;set;}
    public string? VendorReview {get;set;}
    public int? ReviewedByVendorId {get;set;}
    public VendorUser? ReviewedByVendor {get;set;}
    public DateTime? ReviewedDate {get;set;}
    public ReturnRefund? ReturnRefund {get;set;}
}