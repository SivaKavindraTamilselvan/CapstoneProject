namespace Ecommerce.Models;

public class RequestAddOrderDTO
{
    public int AddressId { get; set; }
    public int? CouponId {get;set;}
    public int PaymentMethod {get;set;}
}
public class ResponseAddOrderDTO
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
}

public class ShippingCheckResponseDTO
{
    public decimal TotalShippingCharge { get; set; }
    public bool IsShippingAvailable { get; set; }
}