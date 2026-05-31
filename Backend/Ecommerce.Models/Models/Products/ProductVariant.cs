namespace Ecommerce.Models;
public class ProductVariant
{
    public int ProductVariantId {get;set;}
    public int ProductId {get;set;}
    public Product? Product {get;set;}
    public string Size {get;set;} = string.Empty;
    public string Color {get;set;} = string.Empty;
    public string SKU {get;set;} = string.Empty;
    public int AvailableQuantity {get;set;}
    public decimal Price {get;set;}
    public decimal WeightInKgs {get;set;}
    public decimal LengthInCm {get;set;}
    public decimal WidthInCm {get;set;}
    public int ProductVariantStatusId {get;set;}
    public ProductVariantStatus? ProductVariantStatus {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public ICollection<ProductImage> ProductImages {get;set;}= new List<ProductImage>();
    public ICollection<CartItems> CartItems {get;set;} = new List<CartItems>();
    public ICollection<FavoritesItems> FavoritesItems {get;set;} = new List<FavoritesItems>();
    public ICollection<OrderItems> OrderItems {get;set;} = new List<OrderItems>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}