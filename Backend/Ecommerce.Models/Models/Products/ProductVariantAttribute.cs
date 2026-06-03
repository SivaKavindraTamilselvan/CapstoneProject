namespace Ecommerce.Models;
public class ProductVariantAttribute
{
    public int ProductVariantAttributeId { get; set; }
    public int ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public int ProductSubCategoryAttributeId { get; set; }
    public ProductSubCategoryAttribute? ProductSubCategoryAttribute { get; set; }
    public string AttributeValue { get; set; } = string.Empty;
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}