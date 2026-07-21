namespace Ecommerce.Models;

public class Order
{
    public int OrderId { get; set; }
    public string OrderNumber {get;set;} = string.Empty;
    public int UserId { get; set; }
    public User? Users { get; set; }
    public decimal TotalProductAmount { get; set; } = 0;
    public decimal TotalShippingAmount { get; set; } = 0;
    public decimal TotalCouponAmount { get; set; } = 0;
    public decimal TotalWalletAmount { get; set; } = 0;
    public decimal FinalAmount { get; set; } = 0;
    public int AddressId { get; set; }
    public Address? Address { get; set; }
    public DateTime OrderDate { get; set; }
    public int OrderStatusId { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    public ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
    public ICollection<Payment> Payments {get;set;} = new List<Payment>();
    public ICollection<Shipment> Shipments {get;set;} = new List<Shipment>();
}