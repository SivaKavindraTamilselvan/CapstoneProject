namespace Ecommerce.DTOs;

public class RequestAddReturnDTO
{
    public int ReturnReasonId {get;set;}
    public int OrderItemId {get;set;}
    public string? AdditionalReason {get;set;}
    public int ReturnQuantity {get;set;}
}
public class ResponseAddReturnDTO
{
    public int ReturnId {get;set;}
    public DateTime RequestedDate {get;set;}
}