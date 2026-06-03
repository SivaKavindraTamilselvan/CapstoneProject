namespace Ecommerce.Models;

public class ProductCategory
{
    public int ProductCategoryId { get; set; }
    public string ProductCategoryName { get; set; } = string.Empty;
    public bool IsActive {get;set;} = true;
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public ICollection<ProductSubCategory> ProductSubCategories { get; set; } = new List<ProductSubCategory>();
}