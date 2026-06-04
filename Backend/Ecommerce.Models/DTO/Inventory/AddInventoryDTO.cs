using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;
public class RequestAddInventoryDTO
{
    [Required(ErrorMessage = "Product Variant Id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Variant Id must be greater than 0.")]
    public int ProductVariantId { get; set; }

    [Required(ErrorMessage = "Address Id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Address Id must be greater than 0.")]
    public int AddressId { get; set; }

    [Required(ErrorMessage = "Available Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Available Quantity cannot be negative.")]
    public int AvailableQuantity { get; set; }

    [Required(ErrorMessage = "Reserved Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Reserved Quantity cannot be negative.")]
    public int ReservedQuantity { get; set; }
}
public class ResponseAddInventoryDTO
{
    public int InventoryId { get; set; }
    public int ProductVariantId { get; set; }
    public DateTime UpdatedAt { get; set; }
}