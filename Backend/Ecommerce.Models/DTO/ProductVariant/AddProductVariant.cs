using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestAddProductVariantDTO
{
    [Required(ErrorMessage = "Product Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Id Should Be Greater Than 0")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity Needed For Product")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity Should Be Greater Than 0")]
    public int AvailableQuantity { get; set; }

    [Required(ErrorMessage = "Price Of The Product Is Needed")]
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Weight Of The Product Needed For Shippement")]
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Weight must be greater than 0")]
    public decimal WeightInKgs { get; set; }

    [Required(ErrorMessage = "Length Of The Product Needed For Shippement")]
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Length must be greater than 0")]
    public decimal LengthInCm { get; set; }

    [Required(ErrorMessage = "Width Of The Product Needed For Shippement")]
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Width must be greater than 0")]
    public decimal WidthInCm { get; set; }

    [Required(ErrorMessage = "Height Of The Product Needed For Shippement")]
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Height must be greater than 0")]
    public decimal HeightInCm { get; set; }

    public List<ProductVariantAttributeDTO> requestAddProductVariantAttributeDTOs { get; set; } = new List<ProductVariantAttributeDTO>();

}

public class ProductVariantAttributeDTO
{
    [Required(ErrorMessage = "Attribute Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Id Should Be Greater Than 0")]
    public int ProductSubCategoryAttributeId { get; set; }

    [Required(ErrorMessage = "Attribute Value Is Needed")]
    public string AttributeValue { get; set; } = string.Empty;

}

public class ResponseAddProductVariantDTO
{
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }
}