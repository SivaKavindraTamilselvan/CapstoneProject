using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestAddProductVariantAttributeDTO
{
    public int ProductVariantId { get; set; }
    [Required(ErrorMessage = "Attribute Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Id Should Be Greater Than 0")]
    public int ProductSubCategoryAttributeId { get; set; }

    [Required(ErrorMessage = "Attribute Value Is Needed")]
    public string AttributeValue { get; set; } = string.Empty;

}

public class ResponseAddProductVariantAttributeDTO
{
    public int ProductVariantId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
    public DateTime CreatedAt { get; set; }
}
