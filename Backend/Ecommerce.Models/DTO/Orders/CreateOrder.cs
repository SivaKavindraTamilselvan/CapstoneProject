using Ecommerce.Models;

namespace Ecommerce.DTOs;

public class RequestCreateOrderDTO
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public decimal TotalProductAmount { get; set; } = 0;
    public decimal TotalShippingAmount { get; set; } = 0;
    public decimal TotalCouponAmount { get; set; } = 0;
    public int AddressId { get; set; }
    public bool useWallet {get;set;}

}

// used for order service
public class SelectedCartInventory
{
    public CartItems CartItem { get; set; } = null!;
    public Inventory Inventory { get; set; } = null!;
    public string CourierName {get;set;} = string.Empty;
    public decimal ShippingRate { get; set; }
    public int EstimatedDeliveryDays { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
}