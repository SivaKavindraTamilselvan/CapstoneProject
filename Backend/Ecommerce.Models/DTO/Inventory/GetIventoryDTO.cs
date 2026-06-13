namespace Ecommerce.DTOs;
public class ResponseInventoryDTO
{
    public int InventoryId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
}