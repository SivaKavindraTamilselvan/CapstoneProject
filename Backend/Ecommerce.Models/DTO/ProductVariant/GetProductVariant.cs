namespace Ecommerce.DTOs;
public class ResponseGetAllProductVariant
{
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
    public int MinimuQuantityPerUser {get;set;}

    public List<ResponseProductVariantAttribute> responseProductVariantAttributes {get;set;} = new List<ResponseProductVariantAttribute>();
}