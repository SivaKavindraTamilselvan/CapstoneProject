namespace Ecommerce.DTOs;

using System.ComponentModel.DataAnnotations;

public class RequestUpdateInventoryDTO
{
    [Required(ErrorMessage = "Inventory Id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Inventory Id must be greater than 0.")]
    public int InventoryId { get; set; }

    [Required(ErrorMessage = "Available Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Available Quantity cannot be negative.")]
    public int AvailableQuantity { get; set; }

    public bool UpdateType {get;set;}
}

public class ResponseUpdateInventoryDTO
{
    public int InventoryId { get; set; }
    public int ProductVariantId { get; set; }
    public int AddressId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public DateTime UpdatedAt { get; set; }
}