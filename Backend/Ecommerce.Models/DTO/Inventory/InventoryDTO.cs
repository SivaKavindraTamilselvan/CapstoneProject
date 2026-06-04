namespace Ecommerce.DTOs;

public class RequestAddInventoryDTO
{
    public int ProductVariantId { get; set; }
    public int AddressId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
}

public class ResponseAddInventoryDTO
{
    public int InventoryId { get; set; }
    public int ProductVariantId { get; set; }
    public DateTime UpdatedAt { get; set; }
}