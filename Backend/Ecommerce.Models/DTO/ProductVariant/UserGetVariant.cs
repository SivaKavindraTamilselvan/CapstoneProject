namespace Ecommerce.DTOs;


public class ResponseUserProductVariant
{
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public int MinimumQuantityPerUser { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
    public bool IsReturn { get; set; } = true;
    public bool IsExchange { get; set; } = true;
    public bool isAvailableForSale {get;set;} = true;
    public List<ResponseProductVariantAttributeDTO> Attributes { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}
