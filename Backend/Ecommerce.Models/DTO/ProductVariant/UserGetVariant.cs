namespace Ecommerce.DTOs;


public class ResponseUserProductVariant
{
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public int MinimumQuantityPerUser { get; set; }
    public List<ResponseProductVariantAttributeDTO> Attributes { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}
