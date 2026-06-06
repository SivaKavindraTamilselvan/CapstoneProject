using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestUpdateProductVariantDTO
{
    public int ProductVariantId {get;set;}
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
}

public class ResponseUpdateProductVariantDTO
{
    public int ProductVariantId {get;set;}
    public int ProductId { get; set; }
    public int AvailableQuantity { get; set; }
    public decimal Price { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
    public DateTime UpdatedAt {get;set;}
    public DateTime CreatedAt { get; set; }


}