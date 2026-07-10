namespace Ecommerce.Models;

public class Inventory
{
    public int InventoryId { get; set; }

    public int ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }

    public int AddressId { get; set; }
    public Address? Address { get; set; }
    public int AvailableQuantity { get; set; }

    public int ReservedQuantity { get; set; }

    public bool IsActive {get;set;} = true;

    public DateTime UpdatedAt { get; set; }
    public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
}