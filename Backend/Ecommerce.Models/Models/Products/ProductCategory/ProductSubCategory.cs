namespace Ecommerce.Models;

public class ProductSubCategory
{
    public int ProductSubCategoryId {get;set;}
    public string ProductSubCategoryName {get;set;} = string.Empty;
    public int ProductCategoryId {get;set;}
    public bool IsActive {get;set;} = true;
    public decimal CommissionPercentage { get; set; }
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public ProductCategory? ProductCategory {get;set;}
    public ICollection<Product> Products {get;set;} = new List<Product>();
    public ICollection<ProductSubCategoryAttribute> ProductSubCategoryAttributes { get; set; } = new List<ProductSubCategoryAttribute>();
}