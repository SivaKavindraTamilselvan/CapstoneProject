namespace Ecommerce.DTOs;
public class RequestAddRefundDTO
{
    public int OrderItemsId {get;set;}
    public decimal RefundAmount {get;set;}

}

public class ResponseAddRefundDTO
{
    public int RefundId {get;set;}
    public DateTime RequestedDate {get;set;}

}