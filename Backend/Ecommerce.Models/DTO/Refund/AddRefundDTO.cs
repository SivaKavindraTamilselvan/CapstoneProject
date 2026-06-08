namespace Ecommerce.DTOs;

public class RequestAddReturnRefundDTO
{
    public int RefundTypeId { get; set; }
    public int OrderItemsId { get; set; }
    public int RefundStatusId { get; set; }
    public int ReturnId { get; set; }
    public decimal DamageCost { get; set; }
    public decimal RefundAmount { get; set; }

}

public class RequestAddRefundDTO
{
    public int RefundId { get; set; }
    public int RefundTypeId { get; set; }
    public int OrderItemsId { get; set; }
    public int RefundStatusId { get; set; }
}

public class ResponseAddRefundDTO
{
    public int RefundId { get; set; }
    public DateTime RequestedDate { get; set; }

}