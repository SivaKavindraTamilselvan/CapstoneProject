namespace Ecommerce.Models;

public class ProductImage
{
    public int ProductImageId {get;set;}
    public int ProductId {get;set;}
    public Product? Product {get;set;}
    public int? ProductVariantId {get;set;}
    public ProductVariant? ProductVariant {get;set;}
    public string ImageUrl {get;set;} = string.Empty;
    public int DisplayOrderId {get;set;}
    public DisplayOrder? DisplayOrder {get;set;}
    public bool IsMainImage {get;set;} = false;
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}