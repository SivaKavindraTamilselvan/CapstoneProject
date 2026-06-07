namespace Ecommerce.DTOs;
public class RequestUpdateRefundDTO
{
    public int RefundId {get;set;}
    public int RefundStatusId {get;set;}
    public decimal RefundAmount {get;set;}
}

public class ResponseUpdateRefundDTO
{
    public int RefundId {get;set;}
    public DateTime RequestedDate {get;set;}

}