namespace Ecommerce.DTOs;

public class RequestAddProductVariantDTO
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public decimal Price { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
}

public class ResponseAddProductVariantDTO
{
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}